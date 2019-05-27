using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace endiffo.Search
{
    /// <summary>
    /// SimpleScan is an example implementation of the ISearch interface. In this case, it retrieves environment variables.
    /// </summary>
    internal class SimpleScan : ISearch
    {
        /// <summary>
        /// The environment variables that are currently in use on the system.
        /// </summary>
        private Dictionary<object,object> EnvironmentVariables { get; set; }

        /// <summary>
        /// Copies environment variables into a new dictionary. 
        /// </summary>
        public void GenerateResults()
        {
            EnvironmentVariables = new Dictionary<object, object>();

            foreach (var env in Environment.GetEnvironmentVariables())
            {
                if (env is DictionaryEntry e) EnvironmentVariables.Add(e.Key, e.Value);
            }
        }

        /// <summary>
        /// Uses JsonConvert to create a string of all environment variables.
        /// </summary>
        public string WriteResults()
        {
            try
            {
                return JsonConvert.SerializeObject(EnvironmentVariables);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex);
            }
        }
    }
}
