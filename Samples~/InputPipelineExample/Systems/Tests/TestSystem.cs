using System;
using Unity.Entities;
using UnityEngine;

using groups = RunstarSystems.ECS.Groups;

namespace RunstarSystems.ECS.Sample
{
    public struct RunstarNetworkOrderTestState : IComponentData
    {
        public uint receive_frame;
    }

    /*
    *   Creates a singleton for RunstarNetworkSendOrderTestSystem
    *   to grab.
    *
    *   If groups.RunstarNetworkReceiveGroup is lower priority
    *   This will fail.
    *
    *   You can check out Networks in the Groups folder and
    *   switch Recieve and Send group numbers to test
    */
    [UpdateInGroup(typeof(groups.RunstarNetworkReceiveGroup))]
    public partial class RunstarNetworkReceiveOrderTestSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.HasSingleton<RunstarNetworkOrderTestState>())
            {
                EntityManager.CreateEntity(
                        typeof(RunstarNetworkOrderTestState));

                Debug.Log(
                        "Receive created RunstarNetworkOrderTestState.");
            }

            RunstarNetworkOrderTestState state =
                    SystemAPI.GetSingleton<RunstarNetworkOrderTestState>();

            state.receive_frame++;

            SystemAPI.SetSingleton(state);
        }
    }

    /*
    *   Grabs a singleton created by RunstarNetworkReceiveOrderTestSystem
    *
    *   Fails if this runs before RunstarNetworkReceiveOrderTestSystem
    */
    [UpdateInGroup(typeof(groups.RunstarNetworkSendGroup))]
    public partial class RunstarNetworkSendOrderTestSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            Debug.Log("Runstar send order test system created.");
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.HasSingleton<RunstarNetworkOrderTestState>())
            {
                string message =
                        "Runstar order test failed. " +
                        "RunstarNetworkSendGroup ran before " +
                        "RunstarNetworkReceiveGroup created the test state.";

                Debug.LogError(message);
                throw new InvalidOperationException(message);
            }

            RunstarNetworkOrderTestState state =
                    SystemAPI.GetSingleton<RunstarNetworkOrderTestState>();
        }
    }
}
