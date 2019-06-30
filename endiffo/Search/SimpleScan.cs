using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Endiffo.Search
{
    /// <summary>
    /// SimpleScan is an example implementation of the ISearch interface. In this case, it retrieves environment variables.
    /// </summary>
    internal class SimpleScan : ISearch
    {
        /// <summary>
        /// The filename to save the search result with.
        /// </summary>
        private readonly string Filename;

        /// <summary>
        /// The environment variables that are currently in use on the system.
        /// </summary>
        private Dictionary<object,object> EnvironmentVariables { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename">The filename to save the search result with.</param>
        public SimpleScan(string filename)
        {
            Filename = filename;
        }

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
        /// The filename to save the search result with.
        /// </summary>
        /// <returns>The filename to save the search result with.</returns>
        public string GetFilename()
        {
            return Filename;
        }

        /// <summary>
        /// Uses JsonConvert to create a string of all environment variables.
        /// </summary>
        public Stream WriteResults()
        {
            string jsonString = JsonConvert.SerializeObject(EnvironmentVariables);
            // Todo: Is this the right encoding? Why not UTF8?
            // snippet taken from this page: http://www.csharp411.com/c-convert-string-to-stream-and-stream-to-string/
            byte[] bytesOfString = Encoding.ASCII.GetBytes(jsonString);
            return new MemoryStream(bytesOfString);
        }
    }
}
