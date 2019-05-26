using System;
using System.Collections.Generic;
using System.Text;

namespace endiffo.Search
{
    internal class Result
    {
        internal IDictionary<object, object> Values { get; set; }

        internal ISearch Source { get; set; }

        internal Result()
        {
            Values = new Dictionary<object, object>();
        }
    }
}
