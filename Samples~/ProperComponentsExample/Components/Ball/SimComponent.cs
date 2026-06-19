// using System.ComponentModel;
// using Unity.Entities;
// using Unity.Mathematics;

// /*
// *   These are the two data types we want both in the
// *   editor and in the ecs.
// *   This is the ecs side which is why they have IComponentData
// */
// namespace RunstarSystems.ECS.Components
// {
//     public struct BallSimData : IComponentData
//     {
//         public float mass;
//         public float radius;
//         public float air_density;
//         public float drag_coefficient;
//         public float magnus_coefficient;
//         public float spin_decay;
//         public float max_rpm;
//         public float3 gravity;
//     }

//     public struct BallQuantData : IComponentData
//     {
//         public float position_step;
//         public float velocity_step;
//         public float spin_step;
//     }
// }
