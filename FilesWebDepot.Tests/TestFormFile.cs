using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FilesWebDepot.Tests
{
    public class TestFormFile : IFormFile
    {
        public string FileContent { get; }

        public TestFormFile(string fileName, string fileContent)
        {
            FileName = Name = fileName;
            FileContent = fileContent;
            Length = fileContent.Length;
        }

        public void CopyTo(Stream target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Write(Encoding.UTF8.GetBytes(FileContent));
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = new CancellationToken())
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            await target.WriteAsync(Encoding.UTF8.GetBytes(FileContent), cancellationToken);
        }

        public Stream OpenReadStream() => throw new System.NotImplementedException();

        public string ContentDisposition { get; }
        public string ContentType { get; }
        public string FileName { get; }
        public IHeaderDictionary Headers { get; }
        public long Length { get; }
        public string Name { get; }
    }
}
