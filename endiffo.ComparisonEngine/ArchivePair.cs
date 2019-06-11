using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using endiffo.Comparison.Result;

namespace endiffo.Comparison
{
    public struct ArchivePair
    {
        // These might work as immutable values.
        public ZipArchive Archive1;

        public ZipArchive Archive2;

        public ArchivePair(SnapshotInfoPair fileInfo)
        {
            try
            {
                Archive1 = ZipFile.Open(fileInfo.Snapshot1.FullName, ZipArchiveMode.Read);
                Archive2 = ZipFile.Open(fileInfo.Snapshot2.FullName, ZipArchiveMode.Read);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Archive1 = null;
            Archive2 = null;
        }
    }
}