using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using metadata = RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Organizers
{
    public sealed partial class SubGroupOrganizer
    {
        private static void RegisterGroups<TAttribute>(
                World world,
                ComponentSystemGroup parent_group,
                metadata.RunstarOrganizerContext context)
                where TAttribute : Attribute, IGroupOrderAttribute
        {
            IReadOnlyList<Type> attributed_types =
                    context.TypeRegistry.Get<TAttribute>();

            List<metadata.OrderedGroupInfo> ordered_groups =
                    FindOrderedGroups<TAttribute>(attributed_types);

            RegisterOrderedGroups(
                    world,
                    parent_group,
                    ordered_groups);
        }

        /*
        *   Converts registry results into ordered ComponentSystemGroup entries.
        *
        *   This does not consume/remove anything from Unity's default type list.
        *   The bootstrap/pipeline removes registry-owned unique types centrally.
        */
        private static List<metadata.OrderedGroupInfo> FindOrderedGroups<TAttribute>(
                IReadOnlyList<Type> attributed_types)
                where TAttribute : Attribute, IGroupOrderAttribute
        {
            List<metadata.OrderedGroupInfo> ordered_groups =
                    new List<metadata.OrderedGroupInfo>();

            for (int type_index = 0;
                    type_index < attributed_types.Count;
                    type_index++)
            {
                Type type = attributed_types[type_index];

                if (type == null)
                {
                    continue;
                }

                if (!typeof(ComponentSystemGroup).IsAssignableFrom(type))
                {
                    continue;
                }

                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                if (type.GetCustomAttribute<TAttribute>()
                        is not TAttribute attribute)
                {
                    continue;
                }

                metadata.OrderedGroupInfo group_info =
                        new metadata.OrderedGroupInfo
                        {
                            group_type = type,
                            order = attribute.Order
                        };

                ordered_groups.Add(group_info);
            }

            ordered_groups.Sort(CompareOrderedGroups);

            return ordered_groups;
        }

        private static void RegisterOrderedGroups(
                World world,
                ComponentSystemGroup parent_group,
                List<metadata.OrderedGroupInfo> ordered_groups)
        {
            for (int group_index = 0;
                    group_index < ordered_groups.Count;
                    group_index++)
            {
                Type group_type = ordered_groups[group_index].group_type;

                if (world.GetOrCreateSystemManaged(group_type)
                        is not ComponentSystemGroup child_group)
                {
                    Debug.LogWarning(
                            "Runstar ordered group was not a ComponentSystemGroup: " +
                            group_type.FullName);

                    continue;
                }

                AddGroupToParent(parent_group, child_group);
            }
        }
    }
}
