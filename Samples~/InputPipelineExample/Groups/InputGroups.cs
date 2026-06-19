using Unity.Entities;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Groups
{
    /*
    *   Input is collected before simulation
    */
    [ECSFixedGroupOrder(200)]
    public partial class RunstarInputGroup : ComponentSystemGroup
    {
    }

    /*
    *   Input is gathered from update instead of fixed update
    *   That is where unity collects input so to put it into
    *   the ecs system we also want our collector to be in update
    */
    [ECSUpdateGroupOrder(100)]
    public partial class RunstarInputPullGroup : ComponentSystemGroup
    {
    }
}
