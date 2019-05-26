using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace endiffo.Search
{
    internal class SimpleScan : ISearch
    {
        Result ISearch.GetResult()
        {
            var result = new Result() { Source = this };

            foreach (var env in Environment.GetEnvironmentVariables())
            {
                if (env is DictionaryEntry e) result.Values.Add(e.Key, e.Value);
            }

            return result;
        }
    }
}
