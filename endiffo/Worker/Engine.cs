using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Endiffo.Search;

namespace Endiffo.Worker
{
    /// <summary>
    /// Provides multithreaded functionality for performing multiple search actions against an system.
    /// </summary>
    public class Engine: IDisposable
    {
        /// <summary>
        /// Where the output of the search results will be sent.
        /// </summary>
        private Writer Writer { get; set; }

        /// <summary>
        /// The searches that will be performed.
        /// </summary>
        private Collection Searches { get; set; }

        /// <summary>
        /// Creates a new search engine worker which intialises the searches to be completed. The Parallel class is used to execute them and write the results as they become available.
        /// </summary>
        /// <param name="searches">The searches to perform.</param>
        /// <param name="output">Where to store the output.</param>
        /// <remarks>A better method of sending searches through as arguments is required.</remarks>
        public Engine(string[] searches, List<string> regKeys, string output)
        {
            InitialiseCollection(searches, regKeys);
            Writer = new Writer(output);

            //Foreach of the required searches, generate results and send them to the writer.
            Parallel.ForEach(Searches.AllItems().ToArray(), i =>
            {
                i.GenerateResults();
                Writer.RecieveResult(i);
            });

            //Tell the writer that no more work is expected.
            Writer.WorkIsExpected = false;
            Writer.ResultReady.Set();
        }

        /// <summary>
        /// Adds items to the search collection based on name matches.
        /// </summary>
        /// <param name="searches">The searches to perform</param>
        /// <remarks>This is not an optimal solution for many search options.</remarks>
        private void InitialiseCollection(string[] searches, List<string> regKeys)
        {
            Searches = new Collection();

            if (searches.Contains("SimpleScan")) Searches.TryAdd(new SimpleScan("simple.json"));
            if (searches.Contains("HostsScan")) Searches.TryAdd(new HostsScan("hosts"));

            if (regKeys != null)
            {
                foreach (string regKey in regKeys)
                    Searches.TryAdd(new RegistryScan(regKey, System.Guid.NewGuid() + ".reg"));
            }
        }

        public void Dispose()
        {
            Writer = null;
        }
    }
}
