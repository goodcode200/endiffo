using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

using endiffo;

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
            string encoded = endiffo.Utility.EncodeToBase64(toEncode);
            Assert.AreEqual(encoded, "dGVzdEJhc2U2NA==");
        }
    }
}