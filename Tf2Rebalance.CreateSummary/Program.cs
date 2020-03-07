using System;
using System.Collections.Generic;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Tf2Rebalance.CreateSummary
{
    internal enum FormatterOption
    {
        Rft,
        Text,
        GroupedJson,
    }

    [Command(Name = "CreateSummary", Description = "Creates Summaries from tf2rebalance.txt files")]
    [HelpOption("-? | -h | --help")]
    class Program
    {
        private static ILogger Log = Serilog.Log.ForContext<Program>();

        [Option("-f || --format", Description = "Output format: Rtf, Text or GroupedJson. Defaults to Rtf")]
        public FormatterOption FormatterOption { get; set; } = FormatterOption.Rft;

        [Argument(0, Description = "Enter the filenames (i.e. tf2rebalance.txt)")]
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
            throw new NotSupportedException();
            bool exitSilently = true;
            if (Files.Count == 0)
            {
                exitSilently = false;
                string file = Prompt.GetString("provide a filename");

                Log.Information("manually provided filename: {ManuallyProvidedFilename}", file);
                Files = new[] { file };
            }

            IRebalanceInfoFormater formatter = CreateFormatter(FormatterOption);
            Execute(Files, formatter);

            if (!exitSilently)
            {
                Console.WriteLine("press enter to exit");
                Console.ReadLine();
            }
            return 0;
        }

        private static IRebalanceInfoFormater CreateFormatter(FormatterOption option)
        {
            Log.Debug("using {FormatterOption}", option);
            switch (option)
            {
                case FormatterOption.Rft:
                    return new RebalanceInfoRtfFormater();
                case FormatterOption.Text:
                    return new RebalanceInfoTextFormater();
                case FormatterOption.GroupedJson:
                    return new RebalanceInfoGroupedJsonFormater();
                default:
                    Log.Warning("unknown FormatterOption {FormatterOption}. using RebalanceInfoRtfFormater", option);
                    return new RebalanceInfoRtfFormater();
            }
        }

        private static void Execute(IList<string> files, IRebalanceInfoFormater formatter)
        {
            IDictionary<string, List<ItemInfo>> weaponNames = AlliedModsWiki.GetItemInfos();
            Converter                           converter   = new Converter(weaponNames);

            foreach (string filename in files)
            {
                CreateSummary(filename, converter, formatter);
            }

            Log.Information("finished creating summaries for {ConfigFileCount} configs", files.Count);
        }

        private static void CreateSummary(string filename, Converter converter, IRebalanceInfoFormater formater)
        {
            Log.Information("reading config from {ConfigFileName}", filename);

            string input = File.ReadAllText(filename);

            IEnumerable<RebalanceInfo> rebalanceInfos = converter.Execute(input);
            string output = formater.Create(rebalanceInfos);
            if (string.IsNullOrEmpty(output))
            {
                Log.Error("input could not be read skipping summary");
                return;
            }

            string outputFilename = filename.Replace(Path.GetFileName(filename), Path.GetFileNameWithoutExtension(filename) + "_summary." + formater.FileExtension);

            Log.Information("writing summary to {SummaryFileName}", outputFilename);
            File.WriteAllText(outputFilename, output);
        }
    }
}
