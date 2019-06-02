using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace endiffo.Comparison
{
    public class Engine
    {

        private FileInfo snapshot1;

        private FileInfo snapshot2;

        private ZipArchive archive1;

        private ZipArchive archive2;

        public void Compare(string snapshotFilePath1, string snapshotFilePath2)
        {
            snapshot1 = new FileInfo(snapshotFilePath1);
            snapshot2 = new FileInfo(snapshotFilePath2);

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
                return (snapshot1.Exists && snapshot2.Exists);
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
                archive1 = ZipFile.Open(snapshot1.FullName, ZipArchiveMode.Read);
                archive2 = ZipFile.Open(snapshot2.FullName, ZipArchiveMode.Read);

                return ZipArchiveHasEntries(archive1) && ZipArchiveHasEntries(archive2);
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

            foreach (var entry1 in archive1.Entries)
            {
                var match = archive2.Entries.FirstOrDefault(e => e.Name == entry1.Name);

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
            foreach (var entry2 in archive2.Entries)
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

        private List<Result.ItemDifference> CompareJson(Dictionary<object,object> json1,Dictionary<object,object> json2)
        {
            var differences = new List<Result.ItemDifference>();
            var matchedItems = new HashSet<object>();

            foreach (var item1 in json1)
            {
                if (json2.TryGetValue(item1.Key, out object item2Value))
                {
                    //Compare value
                    if (!item1.Value.Equals(item2Value))
                    {
                        differences.Add(new Result.ItemDifference(item1.Key, item1.Value, item2Value));
                        //Register difference
                    }

                    if (!matchedItems.Contains(item1.Key)) matchedItems.Add(item1.Key);
                }
                else
                {
                    differences.Add(new Result.ItemDifference(item1.Key, item1.Value, null));
                    //Register that item1 entry does not exist in json2 
                }
            }
            foreach (var item2 in json2)
            {
                if (!matchedItems.Contains(item2.Key))
                {
                    //Register that item2 entry does not exist in json1
                    differences.Add(new Result.ItemDifference(item2.Key, null, item2.Value));
                }
            }

            return differences;
        }
    }
}
