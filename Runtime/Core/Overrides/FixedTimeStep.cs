using Unity.Entities;
using UnityEngine;

namespace RunstarSystems.ECS.Overrides
{
    /*
    * Init for Runstar timing.
    *
    * Controls application frame rate and Unity ECS fixed simulation rate.
    */
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class RunstarFixedTimingSystem : SystemBase
    {
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
            Resolution current_resolution = Screen.currentResolution;
            double refresh_rate = current_resolution.refreshRateRatio.value;

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
