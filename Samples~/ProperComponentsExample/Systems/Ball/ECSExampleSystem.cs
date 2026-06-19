using Unity.Burst;
using Unity.Entities;
using UnityEngine;

using data = RunstarSystems.ECS.Components;

/*
*   This system allows use to check if the changed data actually
*   changed on the ecs side
*/
namespace RunstarSystems.ECS.Systems
{
    [BurstCompile]
    // Tells wether this is in fixed or regular update
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BallDebugSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<data.BallSimData>();
            state.RequireForUpdate<data.BallQuantData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                RefRO<data.BallSimData> sim_data,
                RefRO<data.BallQuantData> quant_data)
                in SystemAPI.Query<
                    RefRO<data.BallSimData>,
                    RefRO<data.BallQuantData>>())
            {
                PrintBallSimData(sim_data.ValueRO);
                PrintBallQuantData(quant_data.ValueRO);
            }

            state.Enabled = false;
        }

        private static void PrintBallSimData(data.BallSimData sim_data)
        {
            Debug.Log(
                "BallSimData\n" +
                "mass: " + sim_data.mass + "\n" +
                "radius: " + sim_data.radius + "\n" +
                "air_density: " + sim_data.air_density + "\n" +
                "drag_coefficient: " + sim_data.drag_coefficient + "\n" +
                "magnus_coefficient: " + sim_data.magnus_coefficient + "\n" +
                "spin_decay: " + sim_data.spin_decay + "\n" +
                "max_rpm: " + sim_data.max_rpm + "\n" +
                "gravity: " + sim_data.gravity);
        }

        private static void PrintBallQuantData(data.BallQuantData quant_data)
        {
            Debug.Log(
                "BallQuantData\n" +
                "position_step: " + quant_data.position_step + "\n" +
                "velocity_step: " + quant_data.velocity_step + "\n" +
                "spin_step: " + quant_data.spin_step);
        }
    }
}
