using System;
using System.Collections.Generic;
using System.Text;

namespace endiffo.Comparison.Result
{
    public struct EntryDifference : IDifference
    {
        public enum Type { MissingEntry1, MissingEntry2 }

        public readonly string Name;

        public readonly Type DifferenceType;
    
        public EntryDifference(string name, Type type)
        {
            Name = name;
            DifferenceType = type;
        }

        /// <summary>
        /// Implement a method for printing the differences to a user
        /// </summary>
        void IDifference.PrintDifference()
        {
            throw new NotImplementedException();
        }
    }
}
