using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;
/*
*   Packages the input into a standard format
*/
namespace RunstarSystems.ECS.Systems
{
    [UpdateInGroup(typeof(RunstarInputGroup))]
    public partial class NetworkPacketSystem : SystemBase
    {
        private const float quantized_axis_max = 32767.0f;

        private Entity packet_entity;
        private uint frame;

        protected override void OnCreate()
        {
            base.OnCreate();

            frame = 0;

            packet_entity = EntityManager.CreateEntity(
                    typeof(components.DuelStickClientInputPacket));

            EntityManager.SetComponentData(
                    packet_entity,
                    new components.DuelStickClientInputPacket());
        }

        protected override void OnUpdate()
        {
            frame++;

            components.DuelStickClientInputPacket client_packet =
                    new components.DuelStickClientInputPacket
                    {
                        frame = frame,
                        input_count = 0
                    };

            foreach (components.DualStickInputFrame raw_input
                    in SystemAPI.Query<components.DualStickInputFrame>())
            {
                components.DuelStickInputPacket player_packet =
                        BuildPlayerPacket(raw_input);

                if (raw_input.local_player_index == 0)
                {
                    client_packet.input_0 = player_packet;
                    client_packet.input_count++;
                }

                if (raw_input.local_player_index == 1)
                {
                    client_packet.input_1 = player_packet;
                    client_packet.input_count++;
                }
            }

            EntityManager.SetComponentData(packet_entity, client_packet);
        }

        private static components.DuelStickInputPacket BuildPlayerPacket(
            components.DualStickInputFrame raw_input)
        {
            return new components.DuelStickInputPacket
            {
                local_player_index = raw_input.local_player_index,

                move_x = QuantizeAxis(raw_input.move_x),
                move_y = QuantizeAxis(raw_input.move_y),

                target_x = QuantizeAxis(raw_input.target_x),
                target_y = QuantizeAxis(raw_input.target_y),

                held_actions = raw_input.held_actions,
                pressed_actions = raw_input.pressed_actions,
                released_actions = raw_input.released_actions
            };
        }

        private static short QuantizeAxis(float axis_value)
        {
            float clamped_value = math.clamp(axis_value, -1.0f, 1.0f);
            float scaled_value = math.round(clamped_value * quantized_axis_max);

            return (short)scaled_value;
        }
    }
}
