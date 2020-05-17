using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tf2Rebalance.CreateSummary.Formatter;

namespace Tf2Rebalance.CreateSummary.Tests
{
    [TestClass]
    public class FileSystemTests
    {
        private IRebalanceInfoFormatter _formatter;

        [TestInitialize]
        public void TestInitialize()
        {
            if (Directory.Exists("input"))
                Directory.Delete("input", true);

            if (Directory.Exists("Test"))
                Directory.Delete("Test", true);

            var formatter = new Mock<IRebalanceInfoFormatter>();
            formatter.SetupGet(f => f.FileExtension).Returns("test");
            _formatter = formatter.Object;
        }

        [TestMethod]
        public void NullOutputDirectory()
        {
            FileSystem fileSystem = new FileSystem(_formatter, null);
            fileSystem.WriteToOutput("input\\NullOutputDirectory.txt", "contents");

            Assert.IsTrue(File.Exists("input\\NullOutputDirectory_summary.test"), "File.Exists('input\\NullOutputDirectory.test')");
        }

        [TestMethod]
        public void EmptyOutputDirectory()
        {
            FileSystem fileSystem = new FileSystem(_formatter, String.Empty);
            fileSystem.WriteToOutput("input\\EmptyOutputDirectory.txt", "contents");

            Assert.IsTrue(File.Exists("input\\EmptyOutputDirectory_summary.test"), "File.Exists('input\\EmptyOutputDirectory.test')");
        }

        [TestMethod]
        public void RelativeOutputDirectory()
        {
            FileSystem fileSystem = new FileSystem(_formatter, "Test");
            fileSystem.WriteToOutput("RelativeOutputDirectory.txt", "contents");

            Assert.IsTrue(File.Exists("Test\\RelativeOutputDirectory_summary.test"), "File.Exists('Test\\RelativeOutputDirectory.test')");
        }

        [TestMethod]
        public void AbsoluteOutputDirectory()
        {
            string cwd = Directory.GetCurrentDirectory();
            FileSystem fileSystem = new FileSystem(_formatter, Path.Combine(cwd, "Test"));
            fileSystem.WriteToOutput("AbsoluteOutputDirectory.txt", "contents");

            Assert.IsTrue(File.Exists("Test\\AbsoluteOutputDirectory_summary.test"), "File.Exists('Test\\AbsoluteOutputDirectory.test')");
        }

        [TestMethod]
        public void EnvVariableOutputDirectory()
        {
            string cwd = Directory.GetCurrentDirectory();
            Environment.SetEnvironmentVariable("Tf2Rebalance.CreateSummary.Tests_EnvVariableOutputDirectory_Path", Path.Combine(cwd, "Test"));
            FileSystem fileSystem = new FileSystem(_formatter, "%Tf2Rebalance.CreateSummary.Tests_EnvVariableOutputDirectory_Path%");
            fileSystem.WriteToOutput("EnvVariableOutputDirectory.txt", "contents");

            Assert.IsTrue(File.Exists("Test\\EnvVariableOutputDirectory_summary.test"), "File.Exists('Test\\EnvVariableOutputDirectory_summary.test')");
        }

    }
}