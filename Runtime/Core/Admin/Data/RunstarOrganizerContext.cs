using Unity.Entities;
using registry = RunstarSystems.ECS.Registry;

namespace RunstarSystems.ECS.Data
{
    /*
    *   Connects the registry and inhertance cache to
    *   each defined world
    */
    public sealed class RunstarOrganizerContext
    {
        public World World { get; }

        public registry.TypeRegistry TypeRegistry { get; }

        public registry.InheritCache InheritCache { get; }

        public RunstarOrganizerContext(
                World world,
                registry.TypeRegistry type_registry,
                registry.InheritCache inherit_cache)
        {
            World = world;
            TypeRegistry = type_registry;
            InheritCache = inherit_cache;
        }
    }
}
