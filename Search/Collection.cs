using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace endiffo.Search
{
    /// <summary>
    /// Holds a collection of searches that can be added to and retrieved.
    /// </summary>
    internal class Collection 
    {
        /// <summary>
        /// A dictionary of the searches. It is important to use a dictionary so that uniqueness can be enforced prior to writing to the zip file.
        /// </summary>
        private Dictionary<object, ISearch> Available { get; set; }

        /// <summary>
        /// Creates a new search collection.
        /// </summary>
        internal Collection()
        {
            Available = new Dictionary<object, ISearch>();
        }

        /// <summary>
        /// Uses the available searches and selects the ISearch components.
        /// </summary>
        /// <returns>All searches that have been added to the collection.</returns>
        internal IEnumerable<ISearch> AllItems()
        {
            return Available.Select(a => a.Value);
        }

        /// <summary>
        /// Adds a search item to the available collection. This requires each item to have a unique name.
        /// </summary>
        /// <param name="value">The search item to be added.</param>
        /// <returns>True if is added. False if it already exists.</returns>
        internal bool TryAdd(ISearch value)
        {
            return Available.TryAdd(nameof(ISearch), value);
        }
    }
}
