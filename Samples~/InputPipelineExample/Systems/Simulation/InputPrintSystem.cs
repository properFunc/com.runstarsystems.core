using Unity.Entities;
using UnityEngine;

using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;
/*
*   Prints the input we packaged
*/
namespace RunstarSystems.ECS.Systems
{
    [UpdateInGroup(typeof(RunstarSimulationGroup))]
    public partial class InputPrintSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<components.DuelStickClientInputPacket>();
        }

        protected override void OnUpdate()
        {
            components.DuelStickClientInputPacket client_packet =
                    SystemAPI.GetSingleton<components.DuelStickClientInputPacket>();

            PrintPlayerInput(client_packet.input_0);

            if (client_packet.input_count > 1)
            {
                PrintPlayerInput(client_packet.input_1);
            }
        }

        private static void PrintPlayerInput(
            components.DuelStickInputPacket player_packet)
        {
            Debug.Log(
                    "Player "
                    + player_packet.local_player_index
                    + " Move "
                    + player_packet.move_x
                    + ", "
                    + player_packet.move_y
                    + " Target "
                    + player_packet.target_x
                    + ", "
                    + player_packet.target_y
                    + " Held "
                    + player_packet.held_actions
                    + " Pressed "
                    + player_packet.pressed_actions
                    + " Released "
                    + player_packet.released_actions);
        }
    }
}
