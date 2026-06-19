using Unity.Entities;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Groups
{
    [ECSFixedGroupOrder(300)]
    public partial class RunstarRollbackGroup : ComponentSystemGroup
    {
    }

    /*
    *   We want our simulation group to run after input has been recieved
    *   That way we don't miss a frame
    */
    [ECSFixedGroupOrder(400)]
    public partial class RunstarSimulationGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(500)]
    public partial class RunstarSnapshotGroup : ComponentSystemGroup
    {
    }
}
