using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Pro.Services.File
{
    public interface IFileService
    {
        Task<bool> UploadAvatar(IFormFile formFile, string userId);
    }
}
