using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodilityTest.Task1
{
    public class ConventionTests
    {
        //Scope of assemblies used for testing purposes only, could be changed according to real project structure
        public IList<string> CheckForMissingCommandSuffix()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.Name.EndsWith("Command"))
                .Select(x => x.FullName)
                .ToList();
        }

        //Scope of assemblies used for testing purposes only, could be changed according to real project structure
        public IList<string> CheckForCommandsOutsideNamespace()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.Name.EndsWith("Command") && !x.Namespace.StartsWith("MyApp.Commands"))
                .Select(x => x.FullName)
                .ToList();
        }
    }
}