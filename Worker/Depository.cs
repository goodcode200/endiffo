using endiffo.Search;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace endiffo.Worker
{
    internal sealed class Depository
    {
        private ConcurrentQueue<Result> Results { get; set; }

        internal AutoResetEvent ResultReady { get; set; }
        
        internal Depository()
        {
            Results = new ConcurrentQueue<Result>();
            ResultReady = new AutoResetEvent(false);
        }

        internal void RecieveResult(Result result)
        {
            Results.Enqueue(result);
            ResultReady.Set();
        }
    }
}
