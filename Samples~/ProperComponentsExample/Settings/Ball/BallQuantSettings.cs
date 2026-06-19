using System;
using Unity.Mathematics;
using UnityEngine;

using components = RunstarSystems.ECS.Components;

/*
*   The settings is the data on the editor side
*   This allows you to change the data and automatically
*   bake the new data into the ecs
*/
namespace RunstarSystems.ECS.Settings
{
    [Serializable]
    public struct BallQuantSettings
    {
        public float position_step;
        public float velocity_step;
        public float spin_step;

        /*
        *   The default is going to be in millimeters
        *   This means all significant figures will also be in milliters
        */
        public static BallQuantSettings CreateDefault()
        {
            return new BallQuantSettings
            {
                position_step = 0.001f,
                velocity_step = 0.001f,
                spin_step = 0.001f
            };
        }

        /*
        * Safty here because division will be used
        * Avoid unknown behavior
        */
        public components.BallQuantData ToRuntimeData()
        {
            return new components.BallQuantData
            {
                position_step = math.max(0.000001f, position_step),
                velocity_step = math.max(0.000001f, velocity_step),
                spin_step = math.max(0.000001f, spin_step),
            };
        }
    }
}
