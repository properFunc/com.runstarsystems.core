using Unity.Entities;
namespace RunstarSystems.ECS.Groups
{
    /*
    *   Final level of abstraction for the late update group
    *   Allows LateUpdate connections with the attribute EX: [ECSLateGroupOrder(100)]
    *
    *   @NOTE   the 100 can be any number as it represents the order
    *           for the attribute to work propperly
    */
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class RunstarLatePipelineGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            base.EnableSystemSorting = false;
        }
    }
}
