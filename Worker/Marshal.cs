using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using endiffo.Search;
using endiffo.Worker;
namespace endiffo.Worker
{
    public class Marshal
    {

        private Writer Writer { get; set; }

        public Marshal()
        {
            InitialiseCollection();
            Writer = new Writer();

            var t = Collection.AllItems().ToArray();

            Parallel.ForEach(t, i => Writer.RecieveResult(i.GetResult()));
            Writer.WorkIsExpected = false;
            Writer.ResultReady.Set();

            Thread.Sleep(1000);


            //var searches = new List<Task<endiffo.Search.Result>>();

            //foreach (var item in endiffo.Search.Collection.AllItems)
            //{
            //    searches.Add(Task.Run(() => item.GetResult()));
            //}

            //Task.WaitAll(searches.ToArray());

            var wait = 1;
        }

        private void InitialiseCollection()
        {
            Collection.TryAdd(new SimpleScan());
        }
    }
}
