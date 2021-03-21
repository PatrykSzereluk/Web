using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Pro.Models.Const;

namespace Pro.Helpers
{
    public static class FileHelper
    {
        public static string GetFullFileExtension(this IFormFile file)
        {
            return Regex.Match(file.FileName, RegexConst.GetFullExtension).Value.ToLower();
        }
    }
}
