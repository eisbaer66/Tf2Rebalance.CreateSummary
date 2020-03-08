using System;
using System.Collections.Generic;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Tf2Rebalance.CreateSummary.Formatter;

namespace Tf2Rebalance.CreateSummary
{
    internal enum FormatterOption
    {
        Rft,
        Text,
        Json,
        GroupedJson,
    }

    [Command(Name = "Tf2Rebalance.CreateSummary", Description = "Creates Summaries from tf2rebalance_attributes.txt files.\r\ntry 'Tf2Rebalance.CreateSummary -f:rtf \"C:\\Path to\\tf2rebalance_attributes.txt\"'")]
    [HelpOption("-? | -h | --help")]
    class Program
    {
        private static ILogger Log = Serilog.Log.ForContext<Program>();

        [Option("-f || --format", Description = "Output format: Rtf, Text, Json or GroupedJson. Defaults to Rtf")]
        public FormatterOption FormatterOption { get; set; } = FormatterOption.Rft;

        [Argument(0, Description = "Enter the filenames (i.e. tf2rebalance_attributes.txt)")]
        public IList<string> Files { get; set; }

        static void Main(string[] args)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(new RenderedCompactJsonFormatter(), "log-.clef", restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                CommandLineApplication.Execute<Program>(args);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "fatal exception");
                Console.WriteLine("press enter to exit");
                Console.ReadLine();
                throw;
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        private int OnExecuteAsync(CommandLineApplication app)
        {
            if (Files == null || Files.Count == 0)
            {
                app.ShowHelp();

                Console.WriteLine("press enter to exit");
                Console.ReadLine();
                return 0;
            }

            IRebalanceInfoFormatter formatter = CreateFormatter(FormatterOption);
            Execute(Files, formatter);
            return 0;
        }

        private static IRebalanceInfoFormatter CreateFormatter(FormatterOption option)
        {
            Log.Debug("using {FormatterOption}", option);
            switch (option)
            {
                case FormatterOption.Rft:
                    return new RebalanceInfoRtfFormatter();
                case FormatterOption.Text:
                    return new RebalanceInfoTextFormatter();
                case FormatterOption.Json:
                    return new RebalanceInfoJsonFormatter();
                case FormatterOption.GroupedJson:
                    return new RebalanceInfoGroupedJsonFormatter();
                default:
                    Log.Warning("unknown FormatterOption {FormatterOption}. using RebalanceInfoRtfFormatter", option);
                    return new RebalanceInfoRtfFormatter();
            }
        }

        private static void Execute(IList<string> files, IRebalanceInfoFormatter formatter)
        {
            IDictionary<string, List<ItemInfo>> weaponNames = AlliedModsWiki.GetItemInfos();
            Converter                           converter   = new Converter(weaponNames);

            foreach (string filename in files)
            {
                CreateSummary(filename, converter, formatter);
            }

            Log.Information("finished creating summaries for {ConfigFileCount} configs", files.Count);
        }

        private static void CreateSummary(string filename, Converter converter, IRebalanceInfoFormatter formatter)
        {
            Log.Information("reading config from {ConfigFileName}", filename);

            string input = File.ReadAllText(filename);

            IEnumerable<RebalanceInfo> rebalanceInfos = converter.Execute(input);
            string output = formatter.Create(rebalanceInfos);
            if (string.IsNullOrEmpty(output))
            {
                Log.Error("input could not be read skipping summary");
                return;
            }

            string outputFilename = filename.Replace(Path.GetFileName(filename), Path.GetFileNameWithoutExtension(filename) + "_summary." + formatter.FileExtension);

            Log.Information("writing summary to {SummaryFileName}", outputFilename);
            File.WriteAllText(outputFilename, output);
        }
    }
}
