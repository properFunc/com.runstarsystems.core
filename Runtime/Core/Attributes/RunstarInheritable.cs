using System;
using RunstarSystems.ECS.Data;
namespace RunstarSystems.ECS.Attributes
{
    /*
    *   Base class for Runstar metadata attributes that are allowed to
    *   participate in Runstar inheritance.
    */
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = true,
            Inherited = true)]
    public abstract class RunstarInheritableAttribute : Attribute
    {
        public InheritMode InheritMode { get; }

        protected RunstarInheritableAttribute(
                InheritMode inherit_mode =
                        InheritMode.DirectOnly)
        {
            InheritMode = inherit_mode;
        }
    }
}
