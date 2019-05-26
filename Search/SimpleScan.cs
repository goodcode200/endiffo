using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace endiffo.Search
{
    public class SimpleScan : ISearch
    {
        public Result GetResult()
        {
            var result = new Result();

            foreach (var env in Environment.GetEnvironmentVariables())
            {
                if (env is DictionaryEntry e) result.Values.Add(e.Key, e.Value);
            }
            return result;
        }
    }
}
