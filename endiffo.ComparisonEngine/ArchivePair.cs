using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Endiffo.Comparison.Result;

namespace Endiffo.Comparison
{
    public struct ArchivePair: IDisposable
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
                Archive1 = null;
                Archive2 = null;
                
                Console.WriteLine(ex);
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