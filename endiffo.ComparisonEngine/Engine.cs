using Endiffo.Comparison.Result;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Endiffo.Comparison
{
    public static class Engine
    {
        private static bool SnapshotFilesAreSuitable(ArchivePair pair)
        {
            try
            {
                return ZipArchiveHasEntries(pair.Archive1) && ZipArchiveHasEntries(pair.Archive2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        private static bool ZipArchiveHasEntries(ZipArchive archive)
        {
            return archive?.Entries?.Count > 0;
        }

        private static List<IDifference> MatchEntriesAndCompare(ArchivePair pair)
        {
            var differences = new List<IDifference>();
            var matchedEntries = new HashSet<string>();

            foreach (var entry1 in pair.Archive1.Entries)
            {
                var match = pair.Archive2.Entries.FirstOrDefault(e => e.Name == entry1.Name);

                if (match == null)
                {
                    //entry1 does not exist in archive2, add it to differences
                    differences.Add(new EntryDifference(entry1.Name, EntryDifference.Type.MissingEntry2));
                }
                else
                {
                    //Compare the two entries
                    GoCompare(entry1, match);
                    //Note the matching so archive2 can be processed quicker
                    matchedEntries.Add(entry1.Name);
                }
            }
            foreach (var entry2 in pair.Archive2.Entries)
            {
                if (!matchedEntries.Contains(entry2.Name))
                {
                    //entry2 does not exist in archive1 and must therefore be a difference
                    differences.Add(new EntryDifference(entry2.Name, EntryDifference.Type.MissingEntry1));
                }
            }

            return differences;
        }
        

        private static List<IDifference> GoCompare(ZipArchiveEntry entry1, ZipArchiveEntry entry2)
        {
            if (ManifestExists(entry1))
            {
                return UseManifestToCompare();
            }
            else
            {
                if (TryConvertAsJson(entry1, out Dictionary<object, object> json1) && TryConvertAsJson(entry2, out Dictionary<object, object> json2))
                {
                    return CompareJson(json1, json2);
                }
            }

            return null;
        }

        private static bool ManifestExists(ZipArchiveEntry entry)
        {
            return false;
        }

        /// <summary>
        /// Method not currently supported until a manifest system can be devised.
        /// </summary>
        /// <remarks>
        /// <para>The concept of a manifest is that it will hold details about a particular entry and how to compare it to another entry with the same key.</para></remarks>
        private static List<IDifference> UseManifestToCompare()
        {
            throw new NotImplementedException();
        }

        private static bool TryConvertAsJson(ZipArchiveEntry entry, out Dictionary<object, object> json)
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

        private static List<IDifference> CompareJson(Dictionary<object,object> json1,Dictionary<object,object> json2)
        {
            var differences = new List<IDifference>();

            var matchedItems = new HashSet<object>();

            foreach (var item1 in json1)
            {
                if (json2.TryGetValue(item1.Key, out object item2Value))
                {
                    //Compare value
                    if (!item1.Value.Equals(item2Value))
                    {
                        differences.Add(new ItemDifference(item1.Key, item1.Value, item2Value));
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
