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
    internal class Writer
    {
        private ConcurrentQueue<Result> Results { get; set; }

        internal AutoResetEvent ResultReady { get; set; }

        internal bool WorkIsExpected { private get; set; } = true;

        internal ZipArchive OutputFile { get; set; }

        internal Writer()
        {
            var target = @"C:\Test\output.zip";
            var fileInfo = new FileInfo(target);
            if (fileInfo.Exists) fileInfo.Delete();
            OutputFile = ZipFile.Open(fileInfo.FullName, ZipArchiveMode.Create);
            Results = new ConcurrentQueue<Result>();
            ResultReady = new AutoResetEvent(false);

            var writerThread = new Thread(new ThreadStart(WriteResults));
            writerThread.Start();
        }

        internal void RecieveResult(Result result)
        {
            Results.Enqueue(result);
            ResultReady.Set();
        }

        private void WriteResults()
        {
            try
            {
                while (WorkIsExpected)
                {
                    ResultReady.WaitOne(new TimeSpan(0, 0, 10));

                    if (Results.TryDequeue(out Result result))
                    {
                        WriteResultAsync(result);
                    }
                }
            }
            finally
            {
                OutputFile.Dispose();
            }
        }

        private void WriteResultAsync(Result result)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(result.Values);
            var entry = OutputFile.CreateEntry(nameof(result.Source));

            using (var streamWriter = new StreamWriter(entry.Open()))
            {
                streamWriter.Write(data);
            }
        }
    }
}
