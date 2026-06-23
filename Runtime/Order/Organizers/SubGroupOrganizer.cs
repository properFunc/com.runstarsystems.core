using System;
using System.Collections.Generic;
using System.Reflection;

using Unity.Entities;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using admin = RunstarSystems.ECS.Admin;
using metadata = RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Organizers
{
    /*
    *   Registers Runstar priority-based ECS groups.
    *
    *   Important:
    *
    *   This organizer manually preserves Runstar numeric order.
    *   Do not call SortSystems() on Runstar pipeline groups after adding
    *   ordered children, because Unity's sorter does not know about
    *   Runstar's absolute order attributes.
    *
    *   Group order attributes now derive from ECSGroupOrderAttribute.
    *   Each concrete attribute points to the pipeline group it belongs under.
    */
    [OrganizerPriority(700)]
    public sealed partial class SubGroupOrganizer : admin.IRunstarOrganizer
    {
        public IReadOnlyList<Type> GetAttributeTypes()
        {
            return new Type[]
            {
                typeof(ECSGroupOrderAttribute)
            };
        }

        public void Register(metadata.RunstarOrganizerContext context)
        {
            if (context == null)
            {
                return;
            }

            if (context.World == null)
            {
                return;
            }

            IReadOnlyList<metadata.RegistryMetadata<ECSGroupOrderAttribute>>
                    matches =
                            context.TypeRegistry.GetMatches
                                    <ECSGroupOrderAttribute,
                                     ECSGroupOrderAttribute>();

            Dictionary<Type, List<metadata.OrderedGroupInfo>>
                    ordered_groups_by_pipeline =
                            BuildOrderedGroupsByPipeline(matches);

            foreach (KeyValuePair<Type, List<metadata.OrderedGroupInfo>> pair
                    in ordered_groups_by_pipeline)
            {
                Type pipeline_group_type =
                        pair.Key;

                List<metadata.OrderedGroupInfo> ordered_groups =
                        pair.Value;

                if (pipeline_group_type == null)
                {
                    continue;
                }

                if (ordered_groups == null ||
                        ordered_groups.Count == 0)
                {
                    continue;
                }

                if (!TryCreateAndPlacePipeline(
                        context.World,
                        pipeline_group_type,
                        out ComponentSystemGroup pipeline_group))
                {
                    continue;
                }

                ordered_groups.Sort(CompareOrderedGroups);

                RegisterOrderedGroups(
                        context.World,
                        pipeline_group,
                        ordered_groups);
            }
        }

        private static Dictionary<Type, List<metadata.OrderedGroupInfo>>
                BuildOrderedGroupsByPipeline(
                        IReadOnlyList
                                <metadata.RegistryMetadata
                                        <ECSGroupOrderAttribute>> matches)
        {
            Dictionary<Type, List<metadata.OrderedGroupInfo>>
                    ordered_groups_by_pipeline =
                            new Dictionary
                                    <Type, List<metadata.OrderedGroupInfo>>();

            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata<ECSGroupOrderAttribute> match =
                        matches[match_index];

                Type group_type =
                        match.MatchedType;

                ECSGroupOrderAttribute attribute =
                        match.Metadata;

                if (group_type == null)
                {
                    continue;
                }

                if (attribute == null)
                {
                    continue;
                }

                if (!typeof(ComponentSystemGroup).IsAssignableFrom(group_type))
                {
                    continue;
                }

                if (group_type.IsAbstract || group_type.IsInterface)
                {
                    continue;
                }

                Type pipeline_group_type =
                        attribute.PipelineGroupType;

                if (pipeline_group_type == null)
                {
                    Debug.LogWarning(
                            "Runstar group order attribute had null pipeline group on: " +
                            group_type.FullName);

                    continue;
                }

                if (!typeof(ComponentSystemGroup)
                        .IsAssignableFrom(pipeline_group_type))
                {
                    Debug.LogWarning(
                            "Runstar pipeline group was not a ComponentSystemGroup: " +
                            pipeline_group_type.FullName);

                    continue;
                }

                metadata.OrderedGroupInfo group_info =
                        new metadata.OrderedGroupInfo
                        {
                            group_type = group_type,
                            order = attribute.Order
                        };

                if (!ordered_groups_by_pipeline.TryGetValue(
                        pipeline_group_type,
                        out List<metadata.OrderedGroupInfo> ordered_groups))
                {
                    ordered_groups =
                            new List<metadata.OrderedGroupInfo>();

                    ordered_groups_by_pipeline.Add(
                            pipeline_group_type,
                            ordered_groups);
                }

                ordered_groups.Add(group_info);
            }

            return ordered_groups_by_pipeline;
        }

        private static bool TryCreateAndPlacePipeline(
                World world,
                Type pipeline_group_type,
                out ComponentSystemGroup pipeline_group)
        {
            pipeline_group = null;

            if (world.GetOrCreateSystemManaged(pipeline_group_type)
                    is not ComponentSystemGroup created_pipeline)
            {
                Debug.LogWarning(
                        "Runstar pipeline group was not a ComponentSystemGroup: " +
                        pipeline_group_type.FullName);

                return false;
            }

            pipeline_group = created_pipeline;

            UpdateInGroupAttribute update_in_group =
                    pipeline_group_type.GetCustomAttribute
                            <UpdateInGroupAttribute>(true);

            if (update_in_group == null)
            {
                Debug.LogWarning(
                        "Runstar pipeline group is missing UpdateInGroupAttribute: " +
                        pipeline_group_type.FullName);

                return true;
            }

            Type parent_group_type =
                    update_in_group.GroupType;

            if (parent_group_type == null)
            {
                Debug.LogWarning(
                        "Runstar pipeline group has null UpdateInGroup parent: " +
                        pipeline_group_type.FullName);

                return true;
            }

            if (!typeof(ComponentSystemGroup).IsAssignableFrom(parent_group_type))
            {
                Debug.LogWarning(
                        "Runstar pipeline parent was not a ComponentSystemGroup: " +
                        parent_group_type.FullName);

                return true;
            }

            if (world.GetOrCreateSystemManaged(parent_group_type)
                    is not ComponentSystemGroup parent_group)
            {
                Debug.LogWarning(
                        "Could not create Runstar pipeline parent group: " +
                        parent_group_type.FullName);

                return true;
            }

            AddGroupToParent(
                    parent_group,
                    pipeline_group);

            return true;
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

        /*
        *   Adds the system to the world under parent group.
        */
        private static void AddGroupToParent(
                ComponentSystemGroup parent_group,
                ComponentSystemGroup child_group)
        {
            if (parent_group == null)
            {
                return;
            }

            if (child_group == null)
            {
                return;
            }

            if (parent_group == child_group)
            {
                Debug.LogWarning(
                        "Cannot add ECS group to itself: " +
                        parent_group.GetType().FullName);

                return;
            }

            parent_group.AddSystemToUpdateList(child_group);
        }

        /*
        *   Used for sorting the groups into absolute order.
        */
        private static int CompareOrderedGroups(
                metadata.OrderedGroupInfo left,
                metadata.OrderedGroupInfo right)
        {
            int order_compare =
                    left.order.CompareTo(right.order);

            if (order_compare != 0)
            {
                return order_compare;
            }

            return string.CompareOrdinal(
                    left.group_type.FullName,
                    right.group_type.FullName);
        }
    }
}
