using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;

namespace RunstarSystems.ECS.Admin
{
    /*
    * Searches loaded assemblies and returns usable Type metadata.
    *
    * This scanner does not create systems.
    * It only collects type information for the bootstrap/organizer layer.
    */
    public static class AssemblyScanner
    {
        public static List<Type> GetAllAssemblyTypes()
        {
            List<Type> type_list = new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int assembly_index = 0;
                    assembly_index < assemblies.Length;
                    assembly_index++)
            {
                Type[] types = GetAssemblyTypes(assemblies[assembly_index]);

                for (int type_index = 0;
                        type_index < types.Length;
                        type_index++)
                {
                    type_list.Add(types[type_index]);
                }
            }

            return type_list;
        }

        /*
        *   Catches possible Assembly type reflection issues and
        *   uses an alternative way to search if needed.
        */
        private static Type[] GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                List<Type> valid_types = new List<Type>();

                Type[] loaded_types = exception.Types;

                for (int index = 0; index < loaded_types.Length; index++)
                {
                    if (loaded_types[index] != null)
                    {
                        valid_types.Add(loaded_types[index]);
                    }
                }

                return valid_types.ToArray();
            }
        }
    }
}
