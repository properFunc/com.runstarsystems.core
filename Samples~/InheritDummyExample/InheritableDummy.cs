using System;
using metadata = RunstarSystems.ECS.Data;

/*
*   RunstarInheritableAttribute is a parent
*   attribute in RunstarSystems.ECS.Attributes
*
*   The Dummy attribute calls base to set inhertiable type
*   The Default being DirectOnly for both this and base
*/
namespace RunstarSystems.ECS.Attributes
{
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class InheritableDummyAttribute
            : RunstarInheritableAttribute
    {
        public InheritableDummyAttribute(
                metadata.InheritMode inherit_mode =
                        metadata.InheritMode.DirectOnly)
                : base(inherit_mode)
        {
        }
    }
}
