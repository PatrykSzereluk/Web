using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pro.Helpers;
using Pro.Models.DB;
using Pro.Models.Enums;
using Pro.Services.UserService;

namespace Pro.Services.File
{
    public class FileService : IFileService
    {
        private readonly IUserService _userService;
        private readonly ProContext _context;
        public FileService(IUserService userService, ProContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<bool> UploadAvatar(IFormFile formFile, string userId)
        {
            var extension = formFile.GetFullFileExtension().Trim('.');

             if (extension != ImageExtension.Jpg.ToString().ToLower()
                && extension != ImageExtension.Png.ToString().ToLower())
                return false;

            await using (var ms = new MemoryStream())
            {
                await formFile.CopyToAsync(ms);

                var bytesToScan = ms.ToArray();
            }

            // Scan file

            var folderName = Path.Combine(Directory.GetCurrentDirectory(), "Res");
            Directory.CreateDirectory(folderName);
            var avatarName = Guid.NewGuid();
            var fullPath = Path.Combine(folderName, avatarName + "." + extension);
            try
            {
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

                var userToUpdate = await _userService.GetUserById(int.Parse(userId));

                userToUpdate.AvatarName = avatarName.ToString();

                var updateResult = _context.Users.Update(userToUpdate);

                if (updateResult.State != EntityState.Modified) return false;
                Debug.WriteLine(fullPath);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                System.IO.File.Delete(fullPath);
                Debug.WriteLine(e.Message);
                return false;
            }
            
            return true;
        }
    }
}