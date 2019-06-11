using System;
using System.Collections.Generic;
using System.Text;

namespace Endiffo.Comparison.Result
{
    public struct ItemDifference : IDifference
    {
        public enum Type { Value, MissingValue1, MissingValue2}

        public readonly object Key;

        public readonly object Value1;

        public readonly object Value2;

        public readonly Type DifferenceType;

        public ItemDifference(object key, object value1, object value2)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
            DifferenceType = value1 == null ? Type.MissingValue1 : value2 == null ? Type.MissingValue2 : Type.Value;
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
