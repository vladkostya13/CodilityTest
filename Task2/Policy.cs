using System.Collections.Generic;

namespace CodilityTest.Task2
{
    public class Policy
    {
        public IEnumerable<string> AllowedMethods { get; }
        public IEnumerable<string> AllowedHeaders { get; }
    }
}