using System;
using System.Collections.Generic;
using UnityEngine;

using metadata = RunstarSystems.ECS.Data;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Admin
{
    [OrganizerPriority(500)]
    public sealed class InheritSampleOrganizer : IRunstarOrganizer
    {
        public IReadOnlyList<Type> GetAttributeTypes()
        {
            return new Type[]
            {
                typeof(InheritableDummyAttribute)
            };
        }

        public void Register(metadata.RunstarOrganizerContext context)
        {
            // Gets both the types and the metadata telling
            // if it is a child or parent etc...
            // If you just want the systems you can use Get instead
            IReadOnlyList
                    <metadata.RegistryMetadata<InheritableDummyAttribute>>
                    matches =
                            context.TypeRegistry.GetMatches
                                    <InheritableDummyAttribute,
                                     InheritableDummyAttribute>();

            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata<InheritableDummyAttribute> match =
                        matches[match_index];

                string inherited_text = "";
                if (match.IsInherited)
                {
                    inherited_text = "Child";
                }
                else
                {
                    inherited_text = "Parent";
                }

                Debug.Log(
                        "Inherit sample match: "
                        + inherited_text
                        + "\n | MatchedType: "
                        + match.MatchedType.FullName
                        + "\n | SourceType: "
                        + match.SourceType.FullName
                        + "\n | Mode: "
                        + match.Metadata.InheritMode);
            }
        }
    }
}
