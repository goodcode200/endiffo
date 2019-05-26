using System;
using System.Collections.Generic;
using System.Text;

namespace endiffo.Search
{
    public class Result
    {
        public IDictionary<object, object> Values { get; set; }

        public Result()
        {
            Values = new Dictionary<object, object>();
        }
    }
}
