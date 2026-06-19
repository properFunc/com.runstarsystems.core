using Unity.Entities;
using RunstarSystems.ECS.Attributes;

/*
*   These are some other examples of groups you might have
*   For this example we are not using these groups
*/
namespace RunstarSystems.ECS.Groups
{
    [ECSFixedGroupOrder(100)]
    public partial class RunstarNetworkReceiveGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(600)]
    public partial class RunstarNetworkSendGroup : ComponentSystemGroup
    {
    }
}
