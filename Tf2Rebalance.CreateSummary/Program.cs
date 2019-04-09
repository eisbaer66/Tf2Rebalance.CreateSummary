using System;
using System.Collections.Generic;
using System.IO;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Tf2Rebalance.CreateSummary
{
    class Program
    {
        private static ILogger Log = Serilog.Log.ForContext<Program>();

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
                bool exitSilently = true;
                if (args.Length == 0)
                {
                    exitSilently = false;
                    Console.WriteLine("no argument provided");
                    Console.Write("enter filename: ");
                    string input = Console.ReadLine();

                    Log.Information("manually provided filename: {ManuallyProvidedFilename}", input);
                    args = new[] {input};
                }

                IDictionary<string, List<ItemInfo>> weaponNames = AlliedModsWiki.GetItemInfos();
                Converter converter = new Converter(weaponNames);

                foreach (string filename in args)
                {
                    CreateSummary(filename, converter);
                }

                Log.Information("finished creating summaries for {ConfigFileCount} configs", args.Length);

                if (!exitSilently)
                {
                    Console.WriteLine("press enter to exit");
                    Console.ReadLine();
                }
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

        private static void CreateSummary(string filename, Converter converter)
        {
            Log.Information("reading config from {ConfigFileName}", filename);

            string input = File.ReadAllText(filename);

            string output = converter.Execute(input);
            if (string.IsNullOrEmpty(output))
            {
                Log.Error("input could not be read skipping summary");
                return;
            }

            string outputFilename = filename.Replace(Path.GetFileNameWithoutExtension(filename), Path.GetFileNameWithoutExtension(filename) + "_summary");

            Log.Information("writing summary to {SummaryFileName}", outputFilename);
            File.WriteAllText(outputFilename, output);
        }
    }
}
