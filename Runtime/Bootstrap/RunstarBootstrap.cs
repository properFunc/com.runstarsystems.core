using System;
using System.Linq;
using Unity.Entities;
using System.Collections.Generic;

using metadata = RunstarSystems.ECS.Data;
using registry = RunstarSystems.ECS.Registry;
using pipeline = RunstarSystems.ECS.Admin.RunstarBootStrapPipeline;

namespace RunstarSystems.ECS.Admin
{
    /*
    * Bootstrap for offline ecs
    *
    * Creates a plugin based system for attributes to run,
    * then integrates that with Unity default bootstrap.
    */
    public sealed class RunstarBootstrap : ICustomBootstrap
    {
        // Some organizers want to effect all types
        // before any world filtering happens
        public const int PREFILTER_PRIORITY = 100;

        /*
        *   This works as a wrapper for the pipeline
        *   Is seperated from the pipeline so that a
        *   different custom bootstrap can reuse the code
        *
        /   Does however define the order
        */
        public bool Initialize(string default_world_name)
        {
            World world = new World(default_world_name);
            World.DefaultGameObjectInjectionWorld = world;

            List<Type> system_types =
                    DefaultWorldInitialization
                            .GetAllSystems(WorldSystemFilterFlags.Default)
                            .ToList();

            List<Type> assembly_types =
                    AssemblyScanner.GetAllAssemblyTypes();

            registry.TypeRegistry type_registry =
                    new registry.TypeRegistry();

            registry.InheritCache inherit_cache =
                    new registry.InheritCache();

            List<IRunstarOrganizer> organizers =
                    pipeline.CreateOrganizers(assembly_types);

            metadata.RunstarOrganizerContext context =
                    pipeline.RegisterAttributes(
                            organizers,
                            world,
                            type_registry,
                            inherit_cache,
                            assembly_types);

            pipeline.RunOrganizers(
                    organizers,
                    context,
                    int.MinValue,
                    PREFILTER_PRIORITY);

            pipeline.RemoveRegistryTypes(
                    system_types,
                    context.TypeRegistry);

            pipeline.RunOrganizers(
                    organizers,
                    context,
                    PREFILTER_PRIORITY,
                    pipeline.UNITY_DEFAULT_INSERT_PRIORITY);

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(
                    context.World,
                    system_types);

            pipeline.RunOrganizers(
                    organizers,
                    context,
                    pipeline.UNITY_DEFAULT_INSERT_PRIORITY,
                    int.MaxValue);

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(
                    context.World);

            return true;
        }
    }
}
