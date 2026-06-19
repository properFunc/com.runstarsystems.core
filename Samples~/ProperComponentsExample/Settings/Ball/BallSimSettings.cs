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
    public struct BallSimSettings
    {
        [Header("Body")]
        public float mass;

        public float radius;

        [Header("Air")]
        [Tooltip("Tells how much the air should effect the ball")]
        public float air_density;

        [Tooltip("Controls how quickly the ball slows down in the air")]
        public float drag_coefficient;

        [Tooltip("Controls how strongly spin curves the ball in the air")]
        public float magnus_coefficient;

        [Header("Spin")]
        [Tooltip("How much spin is lost over time")]
        public float spin_decay;

        public float max_rpm;

        public Vector3 gravity;

        // Creating a new settings for Editor and objects
        public static BallSimSettings CreateDefault()
        {
            return new BallSimSettings
            {
                mass = 1.0f,
                radius = 0.25f,
                air_density = 1.225f,
                drag_coefficient = 0.47f,
                magnus_coefficient = 0.08f,
                spin_decay = 0.02f,
                max_rpm = 80.0f,
                gravity = new Vector3(0.0f, -9.80665f, 0.0f),
            };
        }

        // Moving to the ecs
        public components.BallSimData ToRuntimeData()
        {
            return new components.BallSimData
            {
                mass = math.max(0.0001f, mass),
                radius = math.max(0.0001f, radius),
                air_density = math.max(0.0f, air_density),
                drag_coefficient = math.max(0.0f, drag_coefficient),
                magnus_coefficient = magnus_coefficient,
                spin_decay = math.max(0.0f, spin_decay),
                max_rpm = math.max(0.0f, max_rpm),
                gravity = new float3(gravity.x, gravity.y, gravity.z),
            };
        }
    }
}
