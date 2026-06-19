using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;

namespace RunstarSystems.ECS.Components
{
    public struct BallSimData : IComponentData
    {
        public float mass;
        public float radius;
        public float air_density;
        public float drag_coefficient;
        public float magnus_coefficient;
        public float spin_decay;
        public float max_rpm;
        public float3 gravity;
    }

    /*
    *   We need determinisim in our physics
    *   This means keeping track of everything each frame
    *   To make rollback easier
    */
    public struct BallQuantData : IComponentData
    {
        public float position_step;
        public float velocity_step;
        public float spin_step;
    }
}
