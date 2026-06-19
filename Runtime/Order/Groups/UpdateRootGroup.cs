using Unity.Entities;
namespace RunstarSystems.ECS.Groups
{
    /*
    *   Final level of abstraction for the update group
    *   Allows Update connections with the attribute EX: [ECSUpdateGroupOrder(100)]
    *
    *   @NOTE   the 100 can be any number as it represents the order
    *           for the attribute to work propperly
    */
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class RunstarUpdatePipelineGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            base.EnableSystemSorting = false;
        }
    }
}
