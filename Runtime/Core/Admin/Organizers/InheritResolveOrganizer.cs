using System;
using UnityEngine;
using System.Collections.Generic;

using metadata = RunstarSystems.ECS.Data;
using registry = RunstarSystems.ECS.Registry;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Admin.Organizers
{
    /*
    *   This organizer is meant to take in the parent child relationship cache
    *   and use it to declare inheritable types onto the child systems
    *
    *   An example would be a group that only works in client
    *   so you want all the system to inherit that client filter
    */
    [OrganizerPriority(75)]
    public sealed class InheritResolveOrganizer : IRunstarOrganizer
    {
        public IReadOnlyList<Type> GetAttributeTypes()
        {
            return new Type[]
            {
                typeof(InheritFromGroupAttribute),
                typeof(RunstarInheritableAttribute)
            };
        }

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

                CopyMetadata(
                        context,
                        child_type,
                        parent_group_type);
            }
        }

        private static void CopyMetadata(
                metadata.RunstarOrganizerContext context,
                Type child_type,
                Type parent_group_type)
        {
            IReadOnlyList<metadata.RegistryMetadata> parent_matches =
                    context.InheritCache.GetInheritableMatches(
                            parent_group_type);

            for (int parent_match_index = 0;
                    parent_match_index < parent_matches.Count;
                    parent_match_index++)
            {
                metadata.RegistryMetadata parent_match =
                        parent_matches[parent_match_index];

                context.TypeRegistry.AddMatch(
                        parent_match.KeyType,
                        child_type,
                        parent_match.SourceType,
                        parent_match.Metadata,
                        true);
            }
        }
    }
}
