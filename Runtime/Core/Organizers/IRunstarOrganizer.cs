using System;
using System.Collections.Generic;

using RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Admin
{
    /*
    *   A little interface for the plugin system to find
    *   Organizers within the project and allow the type registry to prep
    */
    public interface IRunstarOrganizer
    {
        IReadOnlyList<Type> GetAttributeTypes();

        void Register(RunstarOrganizerContext context);
    }
}
