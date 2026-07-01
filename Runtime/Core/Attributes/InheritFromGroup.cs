using System;

namespace RunstarSystems.ECS.Attributes
{
    [AttributeUsage(
            AttributeTargets.Class | AttributeTargets.Struct,
            AllowMultiple = false,
            Inherited = false)]
    public sealed class InheritFromGroupAttribute : Attribute
    {
        public Type GroupType { get; }

        public InheritFromGroupAttribute(Type group_type)
        {
            GroupType = group_type;
        }
    }
}
