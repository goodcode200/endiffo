using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

using Endiffo;

namespace Endiffo.Test
{
    public class Tests
    {
        /// <summary>
        /// Before all tests are run make sure we have an temporary, empty folder and copy our test config file into it.
        /// </summary>
        [OneTimeSetUp]
        public void BeforeTests()
        {
            if (Directory.Exists(TestConstants.TEMP_FOLDER))
                Directory.Delete(TestConstants.TEMP_FOLDER, true);
            Assert.IsFalse(Directory.Exists(TestConstants.TEMP_FOLDER), "Could not delete existing temporary folder.");

            Directory.CreateDirectory(TestConstants.TEMP_FOLDER);
            File.Copy(
                Constants.DEFAULT_CONFIG_FILENAME,
                Path.Combine (TestConstants.TEMP_FOLDER, Constants.DEFAULT_CONFIG_FILENAME));
            File.Copy(
                TestConstants.CONFIG_FILENAME_2,
                Path.Combine (TestConstants.TEMP_FOLDER, TestConstants.CONFIG_FILENAME_2));
            Directory.SetCurrentDirectory(TestConstants.TEMP_FOLDER);
        }

        /// <summary>
        /// After all tests have been run, navigate out of the temporary test folder and try to delete it.
        /// </summary>
        [OneTimeTearDown]
        public void AfterTests()
        {
            // Directory.SetCurrentDirectory("..");
            // Directory.Delete(TestConstants.TEST_TEMP_FOLDER, true);
            // Assert.IsFalse(Directory.Exists(TestConstants.TEST_TEMP_FOLDER), "Could not delete existing temporary folder.");
        }

        /// <summary>
        /// A common method which just makes sure a given zip archive is present and superficially valid.
        /// </summary>
        /// <param name="zipFilename"></param>
        /// <returns></returns>
        private void ValidateArchive(string zipFilename)
        {
            Assert.IsTrue(File.Exists(zipFilename), "Could not find " + zipFilename);
            
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                List<string> entryNames = archive.Entries.Select(x => x.Name).ToList();
                Assert.Contains("hosts", entryNames, "Zip file does not contain a hosts file.");
                Assert.Contains("simple.json", entryNames, "Zip file does not contain simple.json.");
            }
        }

        /// <summary>
        /// This verifies that we are able to encode a simple base64 string correctly.
        /// </summary>
        [Test]
        public void TestEncodeBase64()
        {
            string toEncode = "testBase64";
            string encoded = Endiffo.Utility.EncodeToBase64(toEncode);
            Assert.AreEqual(encoded, "dGVzdEJhc2U2NA==", "Base64-encoded strings do not match.");
        }

        /// <summary>
        /// This verifies that the ConfigFile structure is compatible with the endiffo.json file.
        /// If not, there might be an issue with file paths,
        /// or one or the other needs to be updated to reflect the most recent modifications.
        /// </summary>
        [Test]
        public void TestLoadConfig()
        {
            var configObject = Utility.ReadConfig(Constants.DEFAULT_CONFIG_FILENAME);
        }

        /// <summary>
        /// Run the main method, then make sure a zip is created, exists in the expected location,
        /// is not corrupted and contains some of the expected files.
        /// </summary>
        [Test]
        public void TestNoArgs()
        {
            (bool noError, string zipFilename) = Program.Run(new string[] {});
            Assert.IsTrue(noError, "Program encountered an error.");
            ValidateArchive(zipFilename);
        }

        /// <summary>
        /// Make sure we can read a user-specified location for the config file.
        /// Does not use different file contents.
        /// </summary>
        [Test]
        public void TestUserSpecifiedConfig()
        {
            void MainMethodBody(string configArg)
            {
                (bool noError, string zipFilename) = Program.Run(new string[] {configArg, TestConstants.CONFIG_FILENAME_2});
                Assert.IsTrue(noError, "Program encountered an error.");
                ValidateArchive(zipFilename);
                File.Delete(zipFilename);
            }

            MainMethodBody("-c");
            MainMethodBody("--config");
        }

        /// <summary>
        /// Make sure we can read a user-specified location for the output.
        /// </summary>
        [Test]
        public void TestUserSpecifiedOutput()
        {
            void MainMethodBody(string outputArg)
            {
                string guid = Guid.NewGuid().ToString();
                string filename = guid + ".zip";
                (bool noError, string zipFilename) = Program.Run(new string[] {outputArg, filename});
                Assert.IsTrue(noError, "Program encountered an error.");
                ValidateArchive(zipFilename);
            }

            MainMethodBody("-o");
            MainMethodBody("--output");
        }

        [Test]
        public void TestParseJsonEnvVars()
        {
            (bool noError, string zipFilename) = Program.Run(new string[] {});
            Assert.IsTrue(noError, "Program encountered an error.");

            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                List<string> entryNames = archive.Entries.Select(x => x.Name).ToList();
                Assert.Contains("simple.json", entryNames, "Zip file does not contain simple.json.");
                
                var entry = archive.GetEntry("simple.json");
                using (var memoryStream = new MemoryStream())
                {
                    entry.Open().CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var fileBytes = memoryStream.ToArray();
                    var jsonString = Encoding.UTF8.GetString(fileBytes, 0, fileBytes.Length);
                    var deserialized = JsonConvert.DeserializeObject<Dictionary<object, object>>(jsonString);
                }
            }
        }
    }
}