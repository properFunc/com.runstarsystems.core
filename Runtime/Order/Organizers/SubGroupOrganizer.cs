using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using group = RunstarSystems.ECS.Groups;
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
    *   ECSFixedGroupOrder / ECSUpdateGroupOrder / ECSLateGroupOrder.
    */
    [OrganizerPriority(700)]
    public sealed partial class SubGroupOrganizer : admin.IRunstarOrganizer
    {
        public IReadOnlyList<Type> GetAttributeTypes()
        {
            return new Type[]
            {
                typeof(ECSFixedGroupOrderAttribute),
                typeof(ECSUpdateGroupOrderAttribute),
                typeof(ECSLateGroupOrderAttribute)
            };
        }

        public void Register(metadata.RunstarOrganizerContext context)
        {
            World world = context.World;

            FixedStepSimulationSystemGroup fixed_root =
                    world.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();

            SimulationSystemGroup update_root =
                    world.GetOrCreateSystemManaged<SimulationSystemGroup>();

            PresentationSystemGroup late_root =
                    world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            group.RunstarFixedPipelineGroup fixed_pipeline =
                    world.GetOrCreateSystemManaged<group.RunstarFixedPipelineGroup>();

            group.RunstarUpdatePipelineGroup update_pipeline =
                    world.GetOrCreateSystemManaged<group.RunstarUpdatePipelineGroup>();

            group.RunstarLatePipelineGroup late_pipeline =
                    world.GetOrCreateSystemManaged<group.RunstarLatePipelineGroup>();

            AddGroupToParent(fixed_root, fixed_pipeline);
            AddGroupToParent(update_root, update_pipeline);
            AddGroupToParent(late_root, late_pipeline);

            RegisterGroups<ECSFixedGroupOrderAttribute>(
                    world,
                    fixed_pipeline,
                    context);

            RegisterGroups<ECSUpdateGroupOrderAttribute>(
                    world,
                    update_pipeline,
                    context);

            RegisterGroups<ECSLateGroupOrderAttribute>(
                    world,
                    late_pipeline,
                    context);
        }

        /*
        *   Adds the system to the world
        *   Under parent group
        */
        private static void AddGroupToParent(
                ComponentSystemGroup parent_group,
                ComponentSystemGroup child_group)
        {
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
        *   Used for sorting the groups into the absolute order
        *
        */
        private static int CompareOrderedGroups(
                metadata.OrderedGroupInfo left,
                metadata.OrderedGroupInfo right)
        {
            int order_compare = left.order.CompareTo(right.order);

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
