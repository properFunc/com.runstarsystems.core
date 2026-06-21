using System;
using System.Collections.Generic;

using metadata = RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Registry
{
    /*
    *   Used to define all types and attributes that will be used
    *   during boot.
    *   General prepise is organziers prep the registry with attributes
    *   The registry will find all types with that attribute
    *
    *   The inheritResolver will store types that inherited an attribute effect
    *   Then the organizers query for all types give the attributes they prepped
    */
    public sealed class TypeRegistry
    {
        private readonly Dictionary<Type, List<metadata.RegistryMetadata>>
                metadata_map;

        public TypeRegistry()
        {
            metadata_map =
                    new Dictionary<Type, List<metadata.RegistryMetadata>>();
        }

        public bool Check<TAttribute>()
                where TAttribute : Attribute
        {
            return Check(typeof(TAttribute));
        }

        public bool Check(Type attribute_type)
        {
            if (attribute_type == null)
            {
                return false;
            }

            return metadata_map.ContainsKey(attribute_type);
        }

        public IReadOnlyList<Type> Get<TAttribute>()
                where TAttribute : Attribute
        {
            return Get(typeof(TAttribute));
        }
        /*
        *   Allows attributes to become markers
        *   Does not get any of the metadata related to the type
        */
        public IReadOnlyList<Type> Get(Type attribute_type)
        {
            IReadOnlyList<metadata.RegistryMetadata> matches =
                    GetMatches(attribute_type);

            if (matches.Count == 0)
            {
                return Array.Empty<Type>();
            }

            List<Type> matched_types = new List<Type>();

            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                Type matched_type = matches[match_index].MatchedType;

                if (matched_type == null)
                {
                    continue;
                }

                if (matched_types.Contains(matched_type))
                {
                    continue;
                }

                matched_types.Add(matched_type);
            }

            return matched_types;
        }

        public IReadOnlyList<metadata.RegistryMetadata>
                GetMatches<TAttribute>()
                where TAttribute : Attribute
        {
            return GetMatches(typeof(TAttribute));
        }

        public IReadOnlyList<metadata.RegistryMetadata> GetMatches(
                Type attribute_type)
        {
            if (attribute_type == null)
            {
                return Array.Empty<metadata.RegistryMetadata>();
            }

            if (!metadata_map.TryGetValue(
                    attribute_type,
                    out List<metadata.RegistryMetadata> matches))
            {
                return Array.Empty<metadata.RegistryMetadata>();
            }

            return matches;
        }

        public IReadOnlyList<metadata.RegistryMetadata<TMetadata>>
                GetMatches<TAttribute, TMetadata>()
                where TAttribute : Attribute
        {
            return GetMatches<TMetadata>(
                    typeof(TAttribute));
        }

        /*
        *   Allows orcastrators to be very specific about what types
        *   they want to grab from the list. Including child/parent data
        */
        public IReadOnlyList<metadata.RegistryMetadata<TMetadata>>
                GetMatches<TMetadata>(Type attribute_type)
        {
            IReadOnlyList<metadata.RegistryMetadata> raw_matches =
                    GetMatches(attribute_type);

            if (raw_matches.Count == 0)
            {
                return Array.Empty<metadata.RegistryMetadata<TMetadata>>();
            }

            List<metadata.RegistryMetadata<TMetadata>> typed_matches =
                    new List<metadata.RegistryMetadata<TMetadata>>();

            for (int match_index = 0;
                    match_index < raw_matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata raw_match =
                        raw_matches[match_index];

                if (raw_match.Metadata is not TMetadata metadata)
                {
                    continue;
                }

                typed_matches.Add(
                        new metadata.RegistryMetadata<TMetadata>(
                                raw_match.MatchedType,
                                raw_match.SourceType,
                                metadata,
                                raw_match.IsInherited));
            }

            return typed_matches;
        }

        /*
        *   Takes and attribute and finds all the direct
        *   types to add to the registry
        *
        *   note the inheritfrom is an attribute
        *   so this will store child types within that
        *   part of the registry to be used later
        *
        *   @NOTE   This also means you can always detect
        *           inhertiable types if needed by grabbing
        *           from the inherittype attribute list
        */
        public void Set<TAttribute>(
                IReadOnlyList<Type> assembly_types)
                where TAttribute : Attribute
        {
            Set(typeof(TAttribute), assembly_types);
        }

        public void Set(
                Type attribute_type,
                IReadOnlyList<Type> assembly_types)
        {
            if (attribute_type == null)
            {
                return;
            }

            if (assembly_types == null)
            {
                return;
            }

            if (!typeof(Attribute).IsAssignableFrom(attribute_type))
            {
                return;
            }

            if (metadata_map.ContainsKey(attribute_type))
            {
                return;
            }

            metadata_map.Add(
                    attribute_type,
                    new List<metadata.RegistryMetadata>());

            for (int type_index = 0;
                    type_index < assembly_types.Count;
                    type_index++)
            {
                Type type = assembly_types[type_index];

                if (type == null)
                {
                    continue;
                }

                IReadOnlyList<Attribute> attributes =
                        FindAttributes(type, attribute_type);

                for (int attribute_index = 0;
                        attribute_index < attributes.Count;
                        attribute_index++)
                {
                    Attribute attribute = attributes[attribute_index];

                    AddMatch(
                            attribute_type,
                            type,
                            type,
                            attribute,
                            false);

                    Type concrete_attribute_type =
                            attribute.GetType();

                    if (concrete_attribute_type == attribute_type)
                    {
                        continue;
                    }

                    AddMatch(
                            concrete_attribute_type,
                            type,
                            type,
                            attribute,
                            false);
                }
            }
        }

        /*
        *   Builds the storable metadata context in the registry
        *   This was added to allow inhertable types as well
        */
        public void AddMatch<TAttribute>(
                Type matched_type,
                Type source_type,
                object metadata,
                bool is_inherited)
                where TAttribute : Attribute
        {
            AddMatch(
                    typeof(TAttribute),
                    matched_type,
                    source_type,
                    metadata,
                    is_inherited);
        }

        public void AddMatch(
                Type attribute_type,
                Type matched_type,
                Type source_type,
                object metadata,
                bool is_inherited)
        {
            if (attribute_type == null)
            {
                return;
            }

            if (matched_type == null)
            {
                return;
            }

            if (source_type == null)
            {
                return;
            }

            if (metadata == null)
            {
                return;
            }

            if (!typeof(Attribute).IsAssignableFrom(attribute_type))
            {
                return;
            }

            if (!metadata_map.TryGetValue(
                    attribute_type,
                    out List<metadata.RegistryMetadata> matches))
            {
                matches = new List<metadata.RegistryMetadata>();
                metadata_map.Add(
                        attribute_type,
                        matches);
            }

            metadata.RegistryMetadata new_match =
                    new metadata.RegistryMetadata(
                            matched_type,
                            source_type,
                            attribute_type,
                            metadata,
                            is_inherited);

            if (ContainsMatch(matches, new_match))
            {
                return;
            }

            matches.Add(new_match);
        }

        /*
        *   Grabs every attribute from a type
        *   And checks to see if the specific
        *   attribute exsists on that type
        */
        public IReadOnlyList<Attribute> FindAttributes(
                Type inspected_type,
                Type attribute_base_type)
        {
            if (inspected_type == null)
            {
                return Array.Empty<Attribute>();
            }

            if (attribute_base_type == null)
            {
                return Array.Empty<Attribute>();
            }

            if (!typeof(Attribute).IsAssignableFrom(attribute_base_type))
            {
                return Array.Empty<Attribute>();
            }

            object[] raw_attributes =
                    inspected_type.GetCustomAttributes(true);

            if (raw_attributes.Length == 0)
            {
                return Array.Empty<Attribute>();
            }

            List<Attribute> matched_attributes =
                    new List<Attribute>();

            for (int attribute_index = 0;
                    attribute_index < raw_attributes.Length;
                    attribute_index++)
            {
                if (raw_attributes[attribute_index]
                        is not Attribute attribute)
                {
                    continue;
                }

                Type concrete_attribute_type =
                        attribute.GetType();

                if (!attribute_base_type.IsAssignableFrom(
                        concrete_attribute_type))
                {
                    continue;
                }

                matched_attributes.Add(attribute);
            }

            return matched_attributes;
        }

        /*
        *   Gets every type within the registry
        *   Heavy function that bruteforce searches through
        *   each attribute type list and ignores duplicates
        */
        public List<Type> GetAllUniqueTypes()
        {
            HashSet<Type> unique_types =
                    new HashSet<Type>();

            foreach (KeyValuePair<Type, List<metadata.RegistryMetadata>> pair
                    in metadata_map)
            {
                List<metadata.RegistryMetadata> matches =
                        pair.Value;

                for (int match_index = 0;
                        match_index < matches.Count;
                        match_index++)
                {
                    Type matched_type =
                            matches[match_index].MatchedType;

                    if (matched_type == null)
                    {
                        continue;
                    }

                    unique_types.Add(matched_type);
                }
            }

            return new List<Type>(unique_types);
        }

        /*
        *   This is a duplicate function serving a slightly different purpose
        *   the duplication is a reminance of splitting the cache and type registry
        *
        *   Could be placed in the context system or in its own static class
        *   but honestly should be fine.
        */
        private static bool ContainsMatch(
                IReadOnlyList<metadata.RegistryMetadata> matches,
                metadata.RegistryMetadata new_match)
        {
            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata existing =
                        matches[match_index];

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
    }
}
