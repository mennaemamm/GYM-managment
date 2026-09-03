using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<string?> UploadAsync(Stream fileStream,string fileName , string folderName , CancellationToken ct=default);

        bool Delete(string fileName, string folderName);
        (Stream stream, string contentType)? GetFile(string fileName, string folderName);
    }
}
