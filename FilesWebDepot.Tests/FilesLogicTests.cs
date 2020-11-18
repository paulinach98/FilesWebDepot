using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using FilesWebDepot.Constants;
using FilesWebDepot.Logic;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace FilesWebDepot.Tests
{
    public class FilesLogicTests
    {
        private const string TestFileName = "TestFileName";
        private const string TestFileContent = "TestFileContent";
        private const string FileEmptyError = "File is empty.";

        private string ExpectedFilePath { get; } = Path.Join(FilesConstants.ResourcesDirectory, TestFileName);

        private readonly IFileSystem _mockFileSystem = new MockFileSystem();

        public FilesLogicTests()
        {
            _mockFileSystem.Directory.CreateDirectory(FilesConstants.ResourcesDirectory);
        }

        [Fact]
        public async Task UploadFile_NewFile_SavedCorrectly()
        {
            var testFile = PrepareFile();
            var filesLogic = new FilesLogic(_mockFileSystem);

            var result = await filesLogic.UploadFile(testFile);

            using (new AssertionScope())
            {
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().Be(testFile.FileName);

                _mockFileSystem.File.Exists(ExpectedFilePath).Should().BeTrue();
                (await _mockFileSystem.File.ReadAllTextAsync(ExpectedFilePath)).Should().Be(TestFileContent);
            }
        }

        [Fact]
        public async Task UploadFile_NewFile0Length_ReturnsFileEmpty()
        {
            var testFile = PrepareFile(fileContent: "");
            var filesLogic = new FilesLogic(_mockFileSystem);

            var result = await filesLogic.UploadFile(testFile);

            using (new AssertionScope())
            {
                result.IsSuccess.Should().BeFalse();
                result.Error.Should().Be(FileEmptyError);

                _mockFileSystem.File.Exists(ExpectedFilePath).Should().BeFalse();
            }
        }

        [Fact]
        public async Task DownloadFile_ExistingFile_ReturnsFile()
        {
            const string fileContent = "ddd";
            const string fileName = "DownloadFileName";
            var filePath = Path.Join(FilesConstants.ResourcesDirectory, fileName);
            await _mockFileSystem.File.AppendAllTextAsync(filePath, fileContent);
            
            var filesLogic = new FilesLogic(_mockFileSystem);

            var result = await filesLogic.DownloadFile(fileName);

            using (new AssertionScope())
            {
                result.IsSuccess.Should().BeTrue();
                result.Value.Length.Should().Be(Encoding.UTF8.GetBytes(fileContent).Length);
            }
        }

        private static IFormFile PrepareFile(string fileName = TestFileName, string fileContent = TestFileContent)
        {
            return new TestFormFile(fileName, fileContent);
        }
    }
}
