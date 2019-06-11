using System;
using System.Collections.Generic;
using System.IO;

using endiffo.Comparison.Result;

namespace endiffo.Comparison
{
    public class SnapshotInfoPair
    {
        public readonly FileInfo Snapshot1;
        public readonly FileInfo Snapshot2;

        public SnapshotInfoPair(string snapshotFilePath1, string snapshotFilePath2)
        {
            var snapshot1 = new FileInfo(snapshotFilePath1);
            var snapshot2 = new FileInfo(snapshotFilePath2);
        }

        public List<IDifference> Compare()
        {
            var differences = new List<IDifference>();

            if (!SnapshotFilesExist())
            {
                Console.WriteLine("Snapshot files do not exist.");
            }

            return differences;
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
    };
}