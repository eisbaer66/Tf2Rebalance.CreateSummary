using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Tf2Rebalance.CreateSummary.Tests
{
    [TestClass]
    public class ConverterTests
    {
        IDictionary<string, string> weaponNames = new Dictionary<string, string>
        {
            {"132", "Eyelander"},
            {"1082", "Festive Eyelander"},
            {"482", "Nessie's Nine Iron"},
            {"266", "Horseless Headless Horsemann\'s Headtaker"},
            {"127", "Direct Hit"},
        };

        [TestInitialize]
        public void TestInitialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        [TestMethod]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.txt")]
        public void TestMethod1(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);
            string expectedOutput = File.ReadAllText(expectedOutputFilename);

            Converter converter = new Converter(weaponNames);


            string output = converter.Execute(input);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
