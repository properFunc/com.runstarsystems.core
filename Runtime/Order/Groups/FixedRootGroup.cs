using Unity.Entities;
namespace RunstarSystems.ECS.Groups
{
    /*
    *   Final level of abstraction for the fixed update group
    *   Allows FixedUpdate connections with the attribute EX: [ECSFixedGroupOrder(100)]
    *
    *   @NOTE   the 100 can be any number as it represents the order
    *           for the attribute to work propperly
    */
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class RunstarFixedPipelineGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            base.EnableSystemSorting = false;
        }
    }
}
