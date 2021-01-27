using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Tf2Rebalance.CreateSummary.Converters;
using Tf2Rebalance.CreateSummary.Converters.Parsers;
using Tf2Rebalance.CreateSummary.Converters.Transformations;
using Tf2Rebalance.CreateSummary.Domain;
using Tf2Rebalance.CreateSummary.Formatter;
using ValveFormat.Superpower;

namespace Tf2Rebalance.CreateSummary.Tests
{
    [TestClass]
    public class Tf2RebalanceConverterTests
    {
        private IDictionary<string, List<ItemInfo>> _itemInfos;

        [TestInitialize]
        public void TestInitialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();

            _itemInfos = AlliedModsWiki.GetItemInfos();
        }

        [TestMethod]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.txt")]
        [DataRow("higps.txt", "higps_summary.txt")]
        [DataRow("higps_withoutClasses.txt", "higps_withoutClasses_summary.txt")]
        public void Text(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);
            string expectedOutput = File.ReadAllText(expectedOutputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new Tf2RebalanceTransformation(new ItemInfoSource(_itemInfos),
                                                                                                          new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoTextFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string output = formatter.Create(rebalanceInfos);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.rtf")]
        [DataRow("higps.txt", "higps_summary.rtf")]
        [DataRow("higps_withoutClasses.txt", "higps_withoutClasses_summary.rtf")]
        [DataRow("additionalFields.txt", "additionalFields_summary.rtf")]
        public void Rtf(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);
            string expectedOutput = File.ReadAllText(expectedOutputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new Tf2RebalanceTransformation(new ItemInfoSource(_itemInfos),
                                                                                                          new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoRtfFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string output = formatter.Create(rebalanceInfos);

            File.WriteAllText("test.rtf", output);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.json")]
        [DataRow("higps.txt", "higps_summary.json")]
        [DataRow("higps_withoutClasses.txt", "higps_withoutClasses_summary.json")]
        [DataRow("additionalFields.txt", "additionalFields_summary.json")]
        public void GroupedJson(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);
            string expectedOutput = File.ReadAllText(expectedOutputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new Tf2RebalanceTransformation(new ItemInfoSource(_itemInfos),
                                                                                                          new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoGroupedJsonFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string output = formatter.Create(rebalanceInfos);

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("additionalFields.txt", "additionalFields_summary_plain.json")]
        public void Json(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);
            string expectedOutput = File.ReadAllText(expectedOutputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new Tf2RebalanceTransformation(new ItemInfoSource(_itemInfos),
                                                                                                          new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoJsonFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string output = formatter.Create(rebalanceInfos);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
