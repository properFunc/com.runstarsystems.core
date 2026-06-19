using System;

namespace RunstarSystems.ECS.Attributes
{
    public interface IGroupOrderAttribute
    {
        int Order { get; }
    }

    /*
    *   @EXAMPLE
    *               [ECSUpdateGroupOrder(100)]
    *               public partial class UpdateExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ECSUpdateGroupOrderAttribute :
            Attribute,
            IGroupOrderAttribute
    {
        public int Order { get; }

        public ECSUpdateGroupOrderAttribute(int groupOrder)
        {
            Order = groupOrder;
        }
    }

    /*
    *   @EXAMPLE
    *               [ECSFixedGroupOrder(100)]
    *               public partial class FixedExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ECSFixedGroupOrderAttribute :
            Attribute,
            IGroupOrderAttribute
    {
        public int Order { get; }

        public ECSFixedGroupOrderAttribute(int groupOrder)
        {
            Order = groupOrder;
        }
    }

    /*
    *   @EXAMPLE
    *               [ECSLateGroupOrder(100)]
    *               public partial class LateExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ECSLateGroupOrderAttribute :
            Attribute,
            IGroupOrderAttribute
    {
        public int Order { get; }

        public ECSLateGroupOrderAttribute(int groupOrder)
        {
            Order = groupOrder;
        }
    }
}
