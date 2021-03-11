using System;
using System.Collections.Generic;
using Pro.Models;
using Pro.Models.Enums;
using Pro.Services.Email;

namespace Pro.Services.UserService
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Models.DB;
    using Models.User;
    using Security.Cryptography;
    using Helpers;

    public class UserService : IUserService
    {
        private readonly ProContext _context;
        private readonly IEncryptor _encryptor;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IEmailNotificationService _emailNotificationService;

        public UserService(IOptions<ApplicationSettings> applicationSettings, IEncryptor encryptor, ProContext context, IEmailNotificationService emailNotificationService)
        {
            _applicationSettings = applicationSettings.Value;
            _encryptor = encryptor;
            _context = context;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<bool> ChangePasswordWithOldPassword(ChangePasswordRequestModel changePasswordRequestModel)
        {

            var user = await _context.Users.FirstOrDefaultAsync(t => t.Id == changePasswordRequestModel.Id);

            if (user == null)
                return false;

            var oldPassword = _encryptor.GetHashPassword(changePasswordRequestModel.OldPassword, _applicationSettings.SaltCount, user.Id);

            if (!user.Password.Equals(oldPassword))
                return false;

            var newPassword = _encryptor.GetHashPassword(changePasswordRequestModel.NewPassword, _applicationSettings.SaltCount, user.Id);

            if (newPassword.Equals(oldPassword))
                return false;

            user.Password = newPassword;

            user.UserHash = user.GetHash();

            var result = _context.Users.Update(user);

            if (result.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            //_emailNotificationService.Create();

            return true;
        }

        public async Task<bool> RemindPasswordSecondStep(RemindPasswordSecondStepRequestModel remindPasswordSecondStepRequestModel)
        {
            var user = await GetUserById(remindPasswordSecondStepRequestModel.Id);

            if (user == null)
                return false;

            if (!user.IsChangingPassword || remindPasswordSecondStepRequestModel.ControlHash != user.ControlHash || remindPasswordSecondStepRequestModel.UserHash != user.UserHash)
                return false;

            user.Password = _encryptor.GetHashPassword(remindPasswordSecondStepRequestModel.NewPassword, _applicationSettings.SaltCount,
                user.Id);

            user.UserHash = user.GetHash();
            user.IsChangingPassword = false;
            user.LastPasswordChanged = DateTime.Now;

            var updateResult = _context.Users.Update(user);

            if (updateResult.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RemindPasswordFirstStep(string loginOrEmail)
        {
            var user = await GetUserByLoginOrEmail(loginOrEmail);

            if (user == null) return false;

            user.IsChangingPassword = true;

            var dbResult = _context.Users.Update(user);

            if (dbResult.State != EntityState.Modified) return false;

            await _context.SaveChangesAsync();
            await _emailNotificationService.SendRemindPasswordEmail(user);
            return true;
        }

        public async Task<User> GetUserByLoginOrEmail(string loginOrEmail, bool isLogin = false)
        {
            if (isLogin)
            {
                var count = await _context.Users.CountAsync(t => t.Email == loginOrEmail || t.Login == loginOrEmail);

                if (count > 1)
                {
                    // block user and send mail to verification
                }

            }
            return await _context.Users.FirstOrDefaultAsync(t => t.Email == loginOrEmail || t.Login == loginOrEmail);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(t => t.Id == id);
        }

    }
}
