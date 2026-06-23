using System;

using group = RunstarSystems.ECS.Groups;

namespace RunstarSystems.ECS.Attributes
{
    public interface IGroupOrderAttribute
    {
        int Order { get; }
    }

    public abstract class ECSGroupOrderAttribute :
            Attribute,
            IGroupOrderAttribute
    {
        public int Order { get; }

        public Type PipelineGroupType { get; }

        protected ECSGroupOrderAttribute(
                int group_order,
                Type pipeline_group_type)
        {
            Order = group_order;
            PipelineGroupType = pipeline_group_type;
        }
    }

    /*
    *   @EXAMPLE
    *               [ECSUpdateGroupOrder(100)]
    *               public partial class UpdateExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ECSUpdateGroupOrderAttribute :
            ECSGroupOrderAttribute
    {
        public ECSUpdateGroupOrderAttribute(
                int group_order)
                : base(
                        group_order,
                        typeof(group.RunstarUpdatePipelineGroup))
        {
        }
    }

    /*
    *   @EXAMPLE
    *               [ECSFixedGroupOrder(100)]
    *               public partial class FixedExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ECSFixedGroupOrderAttribute :
            ECSGroupOrderAttribute
    {
        public ECSFixedGroupOrderAttribute(
                int group_order)
                : base(
                        group_order,
                        typeof(group.RunstarFixedPipelineGroup))
        {
        }
    }

    /*
    *   @EXAMPLE
    *               [ECSLateGroupOrder(100)]
    *               public partial class LateExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ECSLateGroupOrderAttribute :
            ECSGroupOrderAttribute
    {
        public ECSLateGroupOrderAttribute(
                int group_order)
                : base(
                        group_order,
                        typeof(group.RunstarLatePipelineGroup))
        {
        }
    }
}
