using System;

namespace RunstarSystems.ECS.Attributes
{
    /*
    *   Forces organizers to declare their absolute priorty order
    */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class OrganizerPriorityAttribute : Attribute
    {
        public readonly int priority;

        public OrganizerPriorityAttribute(int organizer_priority)
        {
            priority = organizer_priority;
        }
    }
}
