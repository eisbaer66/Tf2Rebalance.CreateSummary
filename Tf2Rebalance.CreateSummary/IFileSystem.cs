using System;
using System.IO;
using Serilog;
using Tf2Rebalance.CreateSummary.Formatter;

namespace Tf2Rebalance.CreateSummary
{
    internal interface IFileSystem
    {
        string Read(string filename);
        void WriteToOutput(string originalFilepath, string content);
    }

    public class FileSystem : IFileSystem
    {
        private static readonly ILogger Logger = Log.ForContext<FileSystem>();
        private readonly IRebalanceInfoFormatter _formatter;
        private readonly string _outputDirectory;

        public FileSystem(IRebalanceInfoFormatter formatter, string outputDirectory)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _outputDirectory = string.IsNullOrEmpty(outputDirectory) 
                                   ? outputDirectory
                                   : Environment.ExpandEnvironmentVariables(outputDirectory);
        }

        public string Read(string filename)
        {
            return File.ReadAllText(filename);
        }

        public void WriteToOutput(string originalFilepath, string content)
        {
            var outputFilename = GetOutputFilename(originalFilepath);

            Log.Information("writing summary to {SummaryFileName}", outputFilename);
            if (!string.IsNullOrEmpty(outputFilename))
            {
                string directory = Path.GetDirectoryName(outputFilename);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);
            }

            File.WriteAllText(outputFilename, content);
        }

        private string GetOutputFilename(string filename)
        {
            string newFilename = Path.GetFileNameWithoutExtension(filename) + "_summary." + _formatter.FileExtension;
            if (string.IsNullOrEmpty(_outputDirectory))
            {
                return filename.Replace(Path.GetFileName(filename), newFilename);
            }

            return Path.Combine(_outputDirectory, newFilename);
        }
    }
}