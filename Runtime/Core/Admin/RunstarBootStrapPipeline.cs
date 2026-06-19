using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

using metadata = RunstarSystems.ECS.Data;
using registry = RunstarSystems.ECS.Registry;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Admin
{
    /*
    *   Seperated from the boostrap so the system can be reused
    *   in custom bootstraps. EX netcode
    */
    public static class RunstarBootStrapPipeline
    {
        public const int UNITY_DEFAULT_INSERT_PRIORITY = 1000;
        /*
        *   Registers the types under the attribute in the registry
        *   Allows those types to be queried later
        */
        public static metadata.RunstarOrganizerContext RegisterAttributes(
                IReadOnlyList<IRunstarOrganizer> organizers,
                World world,
                registry.TypeRegistry type_registry,
                registry.InheritCache inherit_cache,
                IReadOnlyList<Type> assembly_types)
        {
            metadata.RunstarOrganizerContext context =
                    new metadata.RunstarOrganizerContext(
                            world,
                            type_registry,
                            inherit_cache);

            if (organizers == null)
            {
                return context;
            }

            if (assembly_types == null)
            {
                return context;
            }

            for (int organizer_index = 0;
                    organizer_index < organizers.Count;
                    organizer_index++)
            {
                IRunstarOrganizer organizer = organizers[organizer_index];

                if (organizer == null)
                {
                    continue;
                }

                IReadOnlyList<Type> attribute_types =
                        organizer.GetAttributeTypes();

                if (attribute_types == null)
                {
                    continue;
                }

                for (int attribute_index = 0;
                        attribute_index < attribute_types.Count;
                        attribute_index++)
                {
                    Type attribute_type = attribute_types[attribute_index];

                    context.TypeRegistry.Set(
                            attribute_type,
                            assembly_types);

                    context.InheritCache.CacheMatches(
                            context.TypeRegistry,
                            attribute_type);
                }
            }

            return context;
        }

        /*
        *   Runs the sorted organizers from lowest order to highest
        *   context allows the organizers to collect attribute data
        */
        public static void RunOrganizers(
            IReadOnlyList<IRunstarOrganizer> organizers,
            metadata.RunstarOrganizerContext context,
            int min_priority,
            int max_priority)
        {
            // Note that index and priority is not the same
            for (int organizer_index = 0;
                    organizer_index < organizers.Count;
                    organizer_index++)
            {
                IRunstarOrganizer organizer = organizers[organizer_index];

                int priority = GetOrganizerPriority(organizer.GetType());

                if (priority < min_priority)
                {
                    continue;
                }

                if (priority >= max_priority)
                {
                    break;
                }

                organizer.Register(context);
            }
        }

        /*
        *   Builds all the organizers and then sorts them based on priority
        */
        public static List<IRunstarOrganizer> CreateOrganizers(
                IReadOnlyList<Type> type_list)
        {
            List<IRunstarOrganizer> organizers =
                    new List<IRunstarOrganizer>();

            for (int type_index = 0;
                    type_index < type_list.Count;
                    type_index++)
            {
                Type type = type_list[type_index];

                if (!typeof(IRunstarOrganizer).IsAssignableFrom(type))
                {
                    continue;
                }

                if (type.IsInterface || type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type)
                        is not IRunstarOrganizer organizer)
                {
                    Debug.LogWarning(
                            "Could not create Runstar organizer: " +
                            type.FullName);

                    continue;
                }

                organizers.Add(organizer);
            }

            organizers.Sort(CompareOrganizers);

            return organizers;
        }

        /*
        *   Helper funciton used to sort the priorities
        *   In case of tie by name
        */
        private static int CompareOrganizers(
                IRunstarOrganizer left,
                IRunstarOrganizer right)
        {
            int left_priority = GetOrganizerPriority(left.GetType());
            int right_priority = GetOrganizerPriority(right.GetType());

            int priority_compare =
                    left_priority.CompareTo(right_priority);

            if (priority_compare != 0)
            {
                return priority_compare;
            }

            return string.CompareOrdinal(
                    left.GetType().FullName,
                    right.GetType().FullName);
        }

        /*
        *   Helper function to find the priority of the organizer
        *   from the attributes metadata
        */
        private static int GetOrganizerPriority(Type organizer_type)
        {
            if (organizer_type
                    .GetCustomAttribute<OrganizerPriorityAttribute>()
                    is not OrganizerPriorityAttribute attribute)
            {
                return UNITY_DEFAULT_INSERT_PRIORITY;
            }

            return attribute.priority;
        }

        /*
        *   Finds all unique types in the registry and deletes them
        *   in the provided list
        */
        public static void RemoveRegistryTypes(
                List<Type> system_types,
                registry.TypeRegistry type_registry)
        {
            List<Type> registry_types =
                    type_registry.GetAllUniqueTypes();

            HashSet<Type> registry_type_set =
                    new HashSet<Type>(registry_types);

            for (int type_index = system_types.Count - 1;
                    type_index >= 0;
                    type_index--)
            {
                Type system_type = system_types[type_index];

                if (!registry_type_set.Contains(system_type))
                {
                    continue;
                }

                system_types.RemoveAt(type_index);
            }
        }
    }
}
