using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Endiffo.Comparison
{
    public class Engine: IDisposable
    {

        private FileInfo Snapshot1;

        private FileInfo Snapshot2;

        private ZipArchive Archive1;

        private ZipArchive Archive2;

        public void Compare(string snapshotFilePath1, string snapshotFilePath2)
        {
            Snapshot1 = new FileInfo(snapshotFilePath1);
            Snapshot2 = new FileInfo(snapshotFilePath2);

            if (!SnapshotFilesExist())
            {
                Console.WriteLine("Snapshot files do not exist.");
                return;
            }

        }

        private bool SnapshotFilesExist()
        {
            try
            {
                return (Snapshot1.Exists && Snapshot2.Exists);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        private bool SnapshotFilesAreSuitable()
        {
            try
            {
                if (Archive1 != null)
                    Archive1.Dispose();
                if (Archive2 != null)
                    Archive2.Dispose();

                Archive1 = ZipFile.Open(Snapshot1.FullName, ZipArchiveMode.Read);
                Archive2 = ZipFile.Open(Snapshot2.FullName, ZipArchiveMode.Read);
                return ZipArchiveHasEntries(Archive1) && ZipArchiveHasEntries(Archive2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        private bool ZipArchiveHasEntries(ZipArchive archive)
        {
            return archive?.Entries?.Count > 0;
        }

        private void MatchEntriesAndCompare()
        {
            var matchedEntries = new HashSet<string>();

            foreach (var entry1 in Archive1.Entries)
            {
                var match = Archive2.Entries.FirstOrDefault(e => e.Name == entry1.Name);

                if (match == null)
                {
                    //entry1 does not exist in archive2, add it to differences
                }
                else
                {
                    //Compare the two entries
                    GoCompare(entry1, match);
                    //Note the matching so archive2 can be processed quicker
                    matchedEntries.Add(entry1.Name);
                }
            }
            foreach (var entry2 in Archive2.Entries)
            {
                if (!matchedEntries.Contains(entry2.Name))
                {
                    //entry2 does not exist in archive1 and must therefore be a difference
                    //Add to differences
                }
            }
        }

        private void GoCompare(ZipArchiveEntry entry1, ZipArchiveEntry entry2)
        {
            if (ManifestExists(entry1))
            {
                UseManifestToCompare();
                return;
            }
            else
            {
                if (TryConvertAsJson(entry1, out Dictionary<object, object> json1) && TryConvertAsJson(entry2, out Dictionary<object, object> json2))
                {
                    CompareJson(json1, json2);
                }
            }
        }

        private bool ManifestExists(ZipArchiveEntry entry)
        {
            return false;
        }

        private void UseManifestToCompare()
        {
            throw new NotImplementedException();
        }

        private bool TryConvertAsJson(ZipArchiveEntry entry, out Dictionary<object, object> json)
        {
            using (var zipStreamEntry = entry.Open())
            {
                using (var outputStream = new StreamReader(zipStreamEntry))
                {
                    json = JsonConvert.DeserializeObject<Dictionary<object, object>>(outputStream.ReadToEnd());
                }
            }

            return json != null;
        }

        private void CompareJson(Dictionary<object,object> json1,Dictionary<object,object> json2)
        {
            var matchedItems = new HashSet<object>();

            foreach (var item1 in json1)
            {
                if (json2.TryGetValue(item1.Key, out object item2Value))
                {
                    //Compare value
                    if (!item1.Value.Equals(item2Value))
                    {
                        //Register difference
                    }

                    if (!matchedItems.Contains(item1.Key)) matchedItems.Add(item1.Key);
                }
                else
                {
                    //Register that item1 entry does not exist in json2 
                }
            }
            foreach (var item2 in json2)
            {
                if (!matchedItems.Contains(item2.Key))
                {
                    //Register that item2 entry does not exist in json1
                }
            }
            
        }

        public void Dispose()
        {
            if (Archive1 != null)
                Archive1.Dispose();
            if (Archive2 != null)
                Archive2.Dispose();
        }
    }
}
