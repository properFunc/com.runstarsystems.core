using System;
using System.Collections.Generic;
using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Data;

using metadata = RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Registry
{
    /*
    *   A cache for the inheritance system to quickly grab all of the attibutes
    *   from its parent.
    *
    *   Can have future use for organizers to create effects on all inheritable
    *   attributes during boot. Run that organzier before 75 if used in such a way.
    */
    public sealed class InheritCache
    {
        private readonly Dictionary<Type, List<metadata.RegistryMetadata>>
                inheritables_map;

        public InheritCache()
        {
            inheritables_map =
                    new Dictionary<Type, List<metadata.RegistryMetadata>>();
        }

        /*
        *   Stores the relationship of parent to inheritable attributes
        */
        public void CacheMatches(
                TypeRegistry type_registry,
                Type attribute_type)
        {
            if (type_registry == null)
            {
                return;
            }

            if (attribute_type == null)
            {
                return;
            }

            IReadOnlyList<metadata.RegistryMetadata> matches =
                    type_registry.GetMatches(attribute_type);

            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata match = matches[match_index];

                if (match.IsInherited)
                {
                    continue;
                }

                if (match.Metadata is not RunstarInheritableAttribute attribute)
                {
                    continue;
                }

                if (attribute.InheritMode != InheritMode.Inheritable)
                {
                    continue;
                }

                AddInheritableMatch(
                        match.MatchedType,
                        match);
            }
        }

        private void AddInheritableMatch(
                Type source_type,
                metadata.RegistryMetadata match)
        {
            if (source_type == null)
            {
                return;
            }

            if (!inheritables_map.TryGetValue(
                    source_type,
                    out List<metadata.RegistryMetadata> matches))
            {
                matches = new List<metadata.RegistryMetadata>();
                inheritables_map.Add(
                        source_type,
                        matches);
            }

            if (ContainsMatch(matches, match))
            {
                return;
            }

            matches.Add(match);
        }

        /*
        *   Checks to make sure the system hasnt already stored the attibute or type
        *   This can be done much faster but for now is fine
        *   Also skipping types that have already been prepped can save time too
        *   Though can be a little confusing
        */
        private static bool ContainsMatch(
                IReadOnlyList<metadata.RegistryMetadata> matches,
                metadata.RegistryMetadata new_match)
        {
            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata existing = matches[match_index];

                if (existing.MatchedType != new_match.MatchedType)
                {
                    continue;
                }

                if (existing.SourceType != new_match.SourceType)
                {
                    continue;
                }

                if (existing.KeyType != new_match.KeyType)
                {
                    continue;
                }

                if (existing.IsInherited != new_match.IsInherited)
                {
                    continue;
                }

                if (existing.Metadata.GetType()
                        != new_match.Metadata.GetType())
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /*
        *   Get a cached list of attributes from a parent type
        */
        public IReadOnlyList<metadata.RegistryMetadata>
            GetInheritableMatches(Type source_type)
        {
            if (source_type == null)
            {
                return Array.Empty<metadata.RegistryMetadata>();
            }

            if (!inheritables_map.TryGetValue(
                    source_type,
                    out List<metadata.RegistryMetadata> matches))
            {
                return Array.Empty<metadata.RegistryMetadata>();
            }

            return matches;
        }
    }
}
