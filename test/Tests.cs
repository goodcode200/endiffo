using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Endiffo;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Test1()
        {
            string toEncode = "testBase64";
            string encoded = Endiffo.Utility.EncodeToBase64(toEncode);
            Assert.AreEqual(encoded, "dGVzdEJhc2U2NA==");
        }

        /// <summary>
        /// Make sure a zip is created when passing no arguments
        /// </summary>
        [Test]
        public void TestNoArgs()
        {
            (bool noError, string zipFilename) = Program.Run(new string[] {});
            Assert.IsTrue(noError);
            Assert.IsTrue(File.Exists(zipFilename));
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                List<string> entryNames = archive.Entries.Select(x => x.Name).ToList();
                Assert.Contains("hosts", entryNames);
                Assert.Contains("simple2.json", entryNames);
            }
        }

        [Test]
        public void TestZipCreation()
        {
            
        }
    }
}