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
    public class CustomAttributesConverterTests
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
        [DataRow("tf_custom_attributes.txt", "tf_custom_attributes_summary.txt")]
        public void Text(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new CustomAttributesTransformation(new ItemInfoSource(_itemInfos),
                                                                                                              new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoTextFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);

            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt", "tf_custom_attributes_summary.rtf")]
        public void Rtf(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new CustomAttributesTransformation(new ItemInfoSource(_itemInfos),
                                                                                                              new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoRtfFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);

            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt", "tf_custom_attributes_summary.json")]
        public void GroupedJson(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new CustomAttributesTransformation(new ItemInfoSource(_itemInfos),
                                                                                                              new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoGroupedJsonFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);

            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt", "tf_custom_attributes_summary_plain.json")]
        public void Json(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter rebalanceInfoConverter = new GenericConverter<Node>(new ValveFormatParser(),
                                                                           new CustomAttributesTransformation(new ItemInfoSource(_itemInfos),
                                                                                                              new ClassNameSource()));
            IRebalanceInfoFormatter formatter = new RebalanceInfoJsonFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);
            
            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }
    }
}