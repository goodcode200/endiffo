using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace endiffo.Search
{
    internal static class Collection 
    {
        private static Dictionary<object, ISearch> Available { get; set; }

        static Collection()
        {
            Available = new Dictionary<object, ISearch>();
        }

        internal static IEnumerable<ISearch> AllItems()
        {
            return Available.Select(a => a.Value);
        }

        internal static bool TryAdd(ISearch value)
        {
            return Available.TryAdd(nameof(ISearch), value);
        }

        internal static bool TryAdd(object key, ISearch value)
        {
            return Available.TryAdd(key, value);
        }

        internal static bool TryGetValue(object key, out ISearch value)
        {
            return Available.TryGetValue(key, out value);
        }

    }
}
