using System.Collections.Generic;

namespace CodilityTest.Task2
{
    public class CorsPolicy
    {
        public IReadOnlyDictionary<string, Policy> OriginsConfig { get; }
    }
}