using System;
using System.IO;
using System.IO.Compression;

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

        private static bool ZipArchiveHasEntries(ZipArchive archive)
        {
            return archive?.Entries?.Count > 0;
        }
    }
}
