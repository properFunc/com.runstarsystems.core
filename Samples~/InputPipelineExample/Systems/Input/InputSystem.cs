using Unity.Entities;
using UnityEngine.InputSystem;

using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;

/*
*   This collects the input from update
*/
namespace RunstarSystems.ECS.Systems
{
    [UpdateInGroup(typeof(RunstarInputPullGroup))]
    public partial class InputSystem : SystemBase
    {
        private const float KEYBOARD_AXIS_VALUES = 0.8f;
        private const float GAMEPAD_DEADZONE = 0.2f;

        private uint update_frame;

        protected override void OnCreate()
        {
            base.OnCreate();

            update_frame = 0;

            CreateLocalPlayerInput(
                    0,
                    components.InputSourceTypes.keyboard,
                    0);

            CreateLocalPlayerInput(
                    1,
                    components.InputSourceTypes.gamepad,
                    0);
        }

        private void CreateLocalPlayerInput(
                    byte local_player_index,
                    int input_source_type,
                    int input_source_index)
        {
            Entity entity = EntityManager.CreateEntity(
                    typeof(components.DualStickInputFrame),
                    typeof(components.LocalPlayerInput));

            components.DualStickInputFrame raw_input =
                    new components.DualStickInputFrame
                    {
                        local_player_index = local_player_index,
                        update_frame = 0,
                        move_x = 0.0f,
                        move_y = 0.0f,
                        target_x = 0.0f,
                        target_y = 0.0f,
                        held_actions = 0,
                        pressed_actions = 0,
                        released_actions = 0
                    };

            components.LocalPlayerInput binding =
                    new components.LocalPlayerInput
                    {
                        local_player_index = local_player_index,
                        input_source_type = input_source_type,
                        input_source_index = input_source_index
                    };

            EntityManager.SetComponentData(entity, raw_input);
            EntityManager.SetComponentData(entity, binding);
        }

        protected override void OnUpdate()
        {
            update_frame++;

            foreach ((
                    RefRW<components.DualStickInputFrame> raw_input,
                    RefRO<components.LocalPlayerInput> binding)
                    in SystemAPI.Query<
                            RefRW<components.DualStickInputFrame>,
                            RefRO<components.LocalPlayerInput>>())
            {
                components.DualStickInputFrame previous_input =
                        raw_input.ValueRO;

                components.DualStickInputFrame next_input =
                        ReadInputForBinding(binding.ValueRO);

                next_input.local_player_index =
                        binding.ValueRO.local_player_index;

                next_input.update_frame = update_frame;

                //In order for this to work you would need a buffer

                // next_input.pressed_actions =
                //         (ushort)(next_input.held_actions
                //         & ~previous_input.held_actions);

                // next_input.released_actions =
                //         (ushort)(previous_input.held_actions
                //         & ~next_input.held_actions);

                raw_input.ValueRW = next_input;
            }
        }

        private static components.DualStickInputFrame ReadInputForBinding(
                    components.LocalPlayerInput binding)
        {
            if (binding.input_source_type
                    == components.InputSourceTypes.keyboard)
            {
                return ReadKeyboardInput();
            }

            if (binding.input_source_type
                    == components.InputSourceTypes.gamepad)
            {
                return ReadGamepadInput(binding.input_source_index);
            }

            return new components.DualStickInputFrame();
        }

        private static components.DualStickInputFrame ReadKeyboardInput()
        {
            components.DualStickInputFrame raw_input =
                    new components.DualStickInputFrame();

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return raw_input;
            }

            // Player movement keys
            if (keyboard.leftArrowKey.isPressed)
            {
                raw_input.move_x -= KEYBOARD_AXIS_VALUES;
            }

            if (keyboard.rightArrowKey.isPressed)
            {
                raw_input.move_x += KEYBOARD_AXIS_VALUES;
            }

            if (keyboard.downArrowKey.isPressed)
            {
                raw_input.move_y -= KEYBOARD_AXIS_VALUES;
            }

            if (keyboard.upArrowKey.isPressed)
            {
                raw_input.move_y += KEYBOARD_AXIS_VALUES;
            }

            // Player target keys
            if (keyboard.aKey.isPressed)
            {
                raw_input.target_x -= KEYBOARD_AXIS_VALUES;
            }

            if (keyboard.dKey.isPressed)
            {
                raw_input.target_x += KEYBOARD_AXIS_VALUES;
            }

            if (keyboard.sKey.isPressed)
            {
                raw_input.target_y -= KEYBOARD_AXIS_VALUES;
            }

            if (keyboard.wKey.isPressed)
            {
                raw_input.target_y += KEYBOARD_AXIS_VALUES;
            }

            // Special keys example
            if (keyboard.jKey.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.top_spin;
            }

            if (keyboard.kKey.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.back_spin;
            }

            if (keyboard.spaceKey.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.dash;
            }

            if (keyboard.leftShiftKey.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.super;
            }

            return raw_input;
        }

        private static components.DualStickInputFrame ReadGamepadInput(
            int gamepad_index)
        {
            components.DualStickInputFrame raw_input =
                    new components.DualStickInputFrame();

            if (gamepad_index < 0 || gamepad_index >= Gamepad.all.Count)
            {
                return raw_input;
            }

            Gamepad gamepad = Gamepad.all[gamepad_index];

            float move_x = gamepad.leftStick.x.ReadValue();
            float move_y = gamepad.leftStick.y.ReadValue();

            float target_x = gamepad.rightStick.x.ReadValue();
            float target_y = gamepad.rightStick.y.ReadValue();

            raw_input.move_x = ApplyDeadzone(move_x);
            raw_input.move_y = ApplyDeadzone(move_y);

            raw_input.target_x = ApplyDeadzone(target_x);
            raw_input.target_y = ApplyDeadzone(target_y);

            if (gamepad.buttonWest.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.top_spin;
            }

            if (gamepad.buttonEast.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.back_spin;
            }

            if (gamepad.buttonSouth.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.dash;
            }

            if (gamepad.buttonNorth.isPressed)
            {
                raw_input.held_actions |= components.PingPongActionBits.super;
            }

            return raw_input;
        }

        private static float ApplyDeadzone(float axis_value)
        {
            if (axis_value > -GAMEPAD_DEADZONE
                    && axis_value < GAMEPAD_DEADZONE)
            {
                return 0.0f;
            }

            return axis_value;
        }
    }
}
