using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace endiffo.Worker
{
    internal class Writer
    {
        private Depository Depository { get; set; }

        internal Writer(Depository depository)
        {
            Depository = depository;
            var writerThread = new Thread(new ThreadStart(WriteResults));
            writerThread.Start()
        }

        private void WriteResults()
        {

        }
    }
}
