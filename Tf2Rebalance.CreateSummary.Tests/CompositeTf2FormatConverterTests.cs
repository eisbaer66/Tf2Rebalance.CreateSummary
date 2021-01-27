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
    public class CompositeTf2FormatConverterTests
    {
        private IDictionary<string, List<ItemInfo>>       _itemInfos;
        private Dictionary<string, ITransformation<Node>> _transformations;

        [TestInitialize]
        public void TestInitialize()
        {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Verbose()
                         .WriteTo.Console()
                         .Enrich.FromLogContext()
                         .CreateLogger();

            _itemInfos = AlliedModsWiki.GetItemInfos();

            var itemInfoSource  = new ItemInfoSource(_itemInfos);
            var classNameSource = new ClassNameSource();
            _transformations = new Dictionary<string, ITransformation<Node>>
                               {
                                   {
                                       "tf2rebalance_attributes", new Tf2RebalanceTransformation(itemInfoSource,
                                                                                                 classNameSource)
                                   },
                                   {
                                       "Custom Attributes", new CustomAttributesTransformation(itemInfoSource,
                                                                                               classNameSource)
                                   },
                               };
        }

        [TestMethod]
        [DataRow("tf_custom_attributes_error_qualityCont.txt")]
        public void Error(string inputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter              rebalanceInfoConverter = new CompositeTf2FormatConverter(new ValveFormatParser(), _transformations);

            Assert.ThrowsException<InvalidInputException>(() => rebalanceInfoConverter.Execute(input));

        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt",            "tf_custom_attributes_summary.txt")]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.txt")]
        public void Text(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter              rebalanceInfoConverter = new CompositeTf2FormatConverter(new ValveFormatParser(), _transformations);
            IRebalanceInfoFormatter formatter              = new RebalanceInfoTextFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);

            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt",            "tf_custom_attributes_summary.rtf")]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.rtf")]
        public void Rtf(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter              rebalanceInfoConverter = new CompositeTf2FormatConverter(new ValveFormatParser(), _transformations);
            IRebalanceInfoFormatter formatter              = new RebalanceInfoRtfFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);

            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt",            "tf_custom_attributes_summary.json")]
        [DataRow("tf2rebalance_attributes.example.txt", "tf2rebalance_attributes.example_summary.json")]
        public void GroupedJson(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter              rebalanceInfoConverter = new CompositeTf2FormatConverter(new ValveFormatParser(), _transformations);
            IRebalanceInfoFormatter formatter              = new RebalanceInfoGroupedJsonFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);

            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod]
        [DataRow("tf_custom_attributes.txt", "tf_custom_attributes_summary_plain.json")]
        [DataRow("additionalFields.txt",     "additionalFields_summary_plain.json")]
        public void Json(string inputFilename, string expectedOutputFilename)
        {
            string input = File.ReadAllText(inputFilename);

            IConverter              rebalanceInfoConverter = new CompositeTf2FormatConverter(new ValveFormatParser(), _transformations);
            IRebalanceInfoFormatter formatter              = new RebalanceInfoJsonFormatter();

            IEnumerable<RebalanceInfo> rebalanceInfos = rebalanceInfoConverter.Execute(input);
            string                     output         = formatter.Create(rebalanceInfos);
            
            string expectedOutput = File.ReadAllText(expectedOutputFilename);
            Assert.AreEqual(expectedOutput, output);
        }
    }
}