using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using endiffo.Search;
using endiffo.Worker;
namespace Worker
{
    public class Marshal
    {

        private Depository Depository { get; set; }

        public Marshal()
        {
            Depository = new Depository();

            Parallel.ForEach(Collection.AllItems, i => Depository.RecieveResult(i.GetResult()));

            var searches = new List<Task<endiffo.Search.Result>>();

            foreach (var item in endiffo.Search.Collection.AllItems)
            {
                searches.Add(Task.Run(() => item.GetResult()));
            }

            Task.WaitAll(searches.ToArray());

            var wait = 1;
        }

        private void InitialiseCollection()
        {
            Collection.TryAdd(new SimpleScan());
        }
    }
}
