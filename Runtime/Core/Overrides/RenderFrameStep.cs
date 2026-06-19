using Unity.Entities;
using UnityEngine;

namespace RunstarSystems.ECS.Overrides
{
    /*
    *   Init for Runstar timing.
    *
    *   Controls application frame rate and Unity ECS fixed simulation rate.
    *
    *   @NOTE
    *   This is currently a one-shot setup system.
    *   Later this can be replaced with a runtime-configurable timing settings system.
    */
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class RunstarTimingSystem : SystemBase
    {
        /*
        *   This will change to be more customizable later.
        *
        *   For now:
        *       - Render/update frame rate is capped to 120 when refresh rate is higher.
        *       - Fixed simulation runs at 90hz.
        */
        private const int target_update_rate = 120;
        private const float fixed_simulation_rate = 90.0f;

        protected override void OnCreate()
        {
            base.OnCreate();

            ConfigureApplicationFrameRate();
            ConfigureFixedSimulationRate();

            Enabled = false;
        }

        protected override void OnUpdate()
        {
        }

        private static void ConfigureApplicationFrameRate()
        {
#pragma warning disable 0162
            if (target_update_rate <= 0)
            {
                Debug.LogWarning(
                        "Could not configure application frame rate because " +
                        "target_update_rate must be greater than zero.");

                return;
            }
#pragma warning restore 0162

            Resolution current_resolution = Screen.currentResolution;
            double refresh_rate = current_resolution.refreshRateRatio.value;

            if (refresh_rate <= 0.0)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = target_update_rate;

                return;
            }

            if (refresh_rate <= target_update_rate)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;

                return;
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = target_update_rate;
        }

        private void ConfigureFixedSimulationRate()
        {
#pragma warning disable 0162
            if (fixed_simulation_rate <= 0.0f)
            {
                Debug.LogWarning(
                        "Could not configure fixed simulation rate because " +
                        "fixed_simulation_rate must be greater than zero.");

                return;
            }
#pragma warning restore 0162

            FixedStepSimulationSystemGroup fixed_step_group =
                    World.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();

            if (fixed_step_group == null)
            {
                Debug.LogWarning(
                        "Could not configure fixed simulation rate because " +
                        "FixedStepSimulationSystemGroup does not exist.");

                return;
            }

            fixed_step_group.Timestep = 1.0f / fixed_simulation_rate;
        }
    }
}
