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
        [SetUp]
        public void Setup()
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
            // This doesn't work if we comment out the Thread.Sleep.
            Thread.Sleep(1000);
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                List<string> entryNames = archive.Entries.Select(x => x.Name).ToList();
                Assert.Contains("hosts", entryNames);
                Assert.Contains("simple2.json", entryNames);
            }
        }

    }
}