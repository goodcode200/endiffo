using endiffo.Search;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace endiffo.Worker
{
    /// <summary>
    /// Provides the ability to recieve search results from multiple threads and to store them in a zip file.
    /// </summary>
    internal class Writer
    {
        private static object zipLock = new object();

        /// <summary>
        /// Results that are recieved and are to be written.
        /// </summary>
        private ConcurrentQueue<ISearch> Results { get; set; }

        /// <summary>
        /// Used to determine when the next results are ready.
        /// </summary>
        internal AutoResetEvent ResultReady { get; set; }

        /// <summary>
        /// Denotes if further work is expected. This should be set to false when no more operations are expected.
        /// </summary>
        internal bool WorkIsExpected { private get; set; } = true;

        /// <summary>
        /// The zip file used for storing results.
        /// </summary>
        /// <remarks>It may be better to use a third party zip library.</remarks>
        internal ZipArchive OutputFile { get; set; }

        /// <summary>
        /// Creates a new Writer object which prepares the output file and begins a new thread for handling the results.
        /// </summary>
        /// <param name="outputFile"></param>
        internal Writer(string outputFile)
        {
            Results = new ConcurrentQueue<ISearch>();
            ResultReady = new AutoResetEvent(false);
            PrepareOutputFile(outputFile);

            new Thread(new ThreadStart(WriteResults)).Start();
        }

        /// <summary>
        /// By default deletes the output file if it already exists, then creates a new ZipFile.
        /// </summary>
        /// <param name="outputFile">The path of the zip file.</param>
        private void PrepareOutputFile(string outputFile)
        {
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists) fileInfo.Delete();
            OutputFile = ZipFile.Open(fileInfo.FullName, ZipArchiveMode.Create);
        }

        /// <summary>
        /// Recieves results as an ISearch object. The results are added to the queue and ResultReady is notified.
        /// </summary>
        /// <param name="result">The result which will be stored.</param>
        internal void RecieveResult(ISearch result)
        {
            Results.Enqueue(result);
            ResultReady.Set();
        }

        /// <summary>
        /// The worker thread that processes items in the results queue. 
        /// </summary>
        private void WriteResults()
        {
            try
            {
                //WorkIsExpected will be set to false when the Engine finishes its Foreach loop.
                while (WorkIsExpected)
                {
                    //Wait for limited period of time to be notified of results being ready. This may be too short for long running operations.
                    ResultReady.WaitOne(new TimeSpan(0, 0, 10));

                    //If a result can be retrieved from the queue, write the results to file.
                    if (Results.TryDequeue(out ISearch result)) WriteResultAsync(result);
                }
            }
            finally
            {
                //Always dispose of the output file to release locks.
                OutputFile.Dispose();
            }
        }

        /// <summary>
        /// Creates an entry in the zip file using the name of the search results then writes the results to that entry.
        /// </summary>
        /// <param name="result">The result to be written to the zip file.</param>
        private void WriteResultAsync(ISearch result)
        {
            var entry = OutputFile.CreateEntry(result.GetFilename());

            using (var zipEntryStream = entry.Open())
            {   
                result.WriteResults().CopyTo(zipEntryStream);
            }
        }
    }
}
