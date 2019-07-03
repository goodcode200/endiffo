using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

using Endiffo;

namespace Tests
{
    public class Tests
    {
        [OneTimeSetUp]
        public void BeforeTests()
        {
            if (Directory.Exists(Constants.TEMP_TEST_FOLDER))
                Directory.Delete(Constants.TEMP_TEST_FOLDER, true);
            Assert.IsFalse(Directory.Exists(Constants.TEMP_TEST_FOLDER), "Could not delete existing temporary folder.");

            Directory.CreateDirectory(Constants.TEMP_TEST_FOLDER);
            File.Copy(Constants.DEFAULT_CONFIG_FILENAME, Constants.TEMP_TEST_FOLDER);
            Directory.SetCurrentDirectory(Constants.TEMP_TEST_FOLDER);
        }

        [OneTimeTearDown]
        public void AfterTests()
        {
            Directory.SetCurrentDirectory("..");
            Directory.Delete(Constants.TEMP_TEST_FOLDER, true);
            Assert.IsFalse(Directory.Exists(Constants.TEMP_TEST_FOLDER), "Could not delete existing temporary folder.");
        }

        [Test]
        public void Test1()
        {
            string toEncode = "testBase64";
            string encoded = Endiffo.Utility.EncodeToBase64(toEncode);
            Assert.AreEqual(encoded, "dGVzdEJhc2U2NA==", "Base64-encoded strings do not match.");
        }

        /// <summary>
        /// Make sure a zip is created when passing no arguments
        /// </summary>
        [Test]
        public void TestNoArgs()
        {
            (bool noError, string zipFilename) = Program.Run(new string[] {});
            Assert.IsTrue(noError, "Program encountered an error.");
            Assert.IsTrue(File.Exists(zipFilename), "Could not find " + zipFilename);
            
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                List<string> entryNames = archive.Entries.Select(x => x.Name).ToList();
                Assert.Contains("hosts", entryNames, "Zip file does not contain a hosts file.");
                Assert.Contains("simple.json", entryNames, "Zip file does not contain simple.json.");
            }
        }

    }
}