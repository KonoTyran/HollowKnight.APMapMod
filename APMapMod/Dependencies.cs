using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APMapMod
{
    internal static class Dependencies
    {
        public static Dictionary<string, Assembly> strictDependencies = new()
        {
            { "Archipelago.HollowKnight", null },
            { "ItemChanger", null },
            { "MenuChanger", null },
            { "ConnectionMetadataInjector", null },
            { "Vasi", null }
        };

        public static Dictionary<string, Assembly> optionalDependencies = new()
        {
            { "AdditionalMaps", null },
            { "Benchwarp", null },
            { "RandomizableLevers", null },
            { "RandoPlus", null }
        };

        public static void GetDependencies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (strictDependencies.ContainsKey(assembly.GetName().Name))
                {
                    strictDependencies[assembly.GetName().Name] = assembly;
                }

                if (optionalDependencies.ContainsKey(assembly.GetName().Name))
                {
                    optionalDependencies[assembly.GetName().Name] = assembly;
                }
            }
        }

        public static bool HasDependency(string name)
        {
            return (strictDependencies.ContainsKey(name) && strictDependencies[name] != null)
                || (optionalDependencies.ContainsKey(name) && optionalDependencies[name] != null);
        }
    }
}
