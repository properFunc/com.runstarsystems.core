using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using metadata = RunstarSystems.ECS.Data;
using RunstarSystems.ECS.Attributes;


namespace RunstarSystems.ECS.Admin.Organizers
{
    /*
    *   Places all types that use the [InheritFromGroup(<parent>)] attribute
    *   under the parent defined in the parent metadata above
    */
    [OrganizerPriority(2500)]
    public sealed class InheritPlacementOrganizer : IRunstarOrganizer
    {
        /*
        *   Preps the registry with attribute requirements
        *
        *   @NOTE   Though this is fine and will work just fine
        *           It is technically not needed. And could save time by not
        *           putting anything because it is done in the resolve organizer
        */
        public IReadOnlyList<Type> GetAttributeTypes()
        {
            return new Type[]
            {
                typeof(InheritFromGroupAttribute)
            };
        }

        /*
        *   Runs through each inherited system and places it under its parent
        */
        public void Register(metadata.RunstarOrganizerContext context)
        {
            if (context == null)
            {
                return;
            }

            IReadOnlyList<metadata.RegistryMetadata<InheritFromGroupAttribute>>
                    inherit_matches =
                            context.TypeRegistry.GetMatches
                                    <InheritFromGroupAttribute,
                                     InheritFromGroupAttribute>();

            for (int match_index = 0;
                    match_index < inherit_matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata<InheritFromGroupAttribute> inherit_match =
                        inherit_matches[match_index];

                Type child_type = inherit_match.MatchedType;
                Type parent_group_type = inherit_match.Metadata.GroupType;

                if (child_type == null)
                {
                    continue;
                }

                if (parent_group_type == null)
                {
                    Debug.LogError(
                            "InheritFromGroup has null parent group on: "
                            + child_type.FullName);

                    continue;
                }

                AddToParent(
                        context.World,
                        child_type,
                        parent_group_type);
            }
        }

        private static void AddToParent(
                World world,
                Type child_type,
                Type parent_group_type)
        {
            if (world == null)
            {
                return;
            }

            if (!typeof(ComponentSystemGroup)
                    .IsAssignableFrom(parent_group_type))
            {
                Debug.LogError(
                        "InheritFromGroup parent must be a ComponentSystemGroup: "
                        + parent_group_type.FullName);

                return;
            }

            ComponentSystemBase parent_system =
                    world.GetExistingSystemManaged(parent_group_type);

            if (parent_system is not ComponentSystemGroup parent_group)
            {
                Debug.LogError(
                        "Could not find parent group in world: "
                        + parent_group_type.FullName);

                return;
            }

            bool is_managed_system =
                    typeof(ComponentSystemBase).IsAssignableFrom(child_type);

            bool is_unmanaged_system =
                    typeof(ISystem).IsAssignableFrom(child_type);

            if (!is_managed_system && !is_unmanaged_system)
            {
                Debug.LogError(
                        "InheritFromGroup child must be a ComponentSystemBase or ISystem: "
                        + child_type.FullName);

                return;
            }

            if (is_managed_system)
            {
                ComponentSystemBase child_system =
                        world.GetOrCreateSystemManaged(child_type);

                if (child_system == null)
                {
                    Debug.LogError(
                            "Could not create child system: "
                            + child_type.FullName);

                    return;
                }

                if (parent_group == child_system)
                {
                    Debug.LogError(
                            "Cannot add ECS group/system to itself: "
                            + child_type.FullName);

                    return;
                }

                parent_group.AddSystemToUpdateList(child_system);
                return;
            }

            SystemHandle child_handle =
                    world.GetOrCreateSystem(child_type);

            if (child_handle == SystemHandle.Null)
            {
                Debug.LogError(
                        "Could not create child system: "
                        + child_type.FullName);

                return;
            }

            parent_group.AddSystemToUpdateList(child_handle);
        }
    }
}
