using RunstarSystems.ECS.Admin;
using Unity.Entities;
using UnityEngine;
using RunstarSystems.ECS.Attributes;
using metadata = RunstarSystems.ECS.Data;

/*
*   In this example we have a group with a dummy attribute
*   [InheritableDummy(InheritMode.Inheritable)]
*
*   Here we are wanting show that teh Dummy trait was passed
*   to the system that inherited it.
*/
namespace RunstarSystems.ECS.Samples
{
    // Inheritbale means pass to child
    // Feel free to change to DirectOnly and see what changes
    [InheritableDummy(metadata.InheritMode.Inheritable)]
    [ECSLateGroupOrder(500)]
    public partial class InheritExampleGroup : ComponentSystemGroup // Parent
    {
    }

    /*
    *   The [InheritFromGroup] attribute has two effects
    *   Effect one all inheritable attibutes like inheritdummy
    *   Are passed to the system
    *
    *   The second is it update in the same group as the parent
    *   think of it doubling as the [UpdateInGroup()] from Unity
    */
    [InheritFromGroup(typeof(InheritExampleGroup))] // inherit Dummy
    public partial class InheritExampleSystem : SystemBase // Child
    {
        private bool has_logged;
        // Should update under InheritExampleGroup
        protected override void OnUpdate()
        {
            if (has_logged)
            {
                return;
            }

            has_logged = true;

            Debug.Log("InheritExampleSystem updated through " +
                      "InheritFromGroup placement.");
        }
    }
}
