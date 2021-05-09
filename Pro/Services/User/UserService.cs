namespace Pro.Services.User
{
    using System;
    using Email;
    using Pro.Services.UserService;
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

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(t => t.Id == id);
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

            return await ChangePassword(user, newPassword);
        }

        private async Task<bool> ChangePassword(User user, string newPassword)
        {
            user.Password = newPassword;

            user.UserHash = user.GetHash();

            user.LastPasswordChanged = DateTime.Now;

            var result = _context.Users.Update(user);

            if (result.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            //_emailNotificationService.Create();

            return true;
        }

        public async Task<bool> RemindPasswordSecondStep(RemindPasswordSecondStepRequestModel remindPasswordSecondStepRequestModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(t =>
                t.ControlHash == remindPasswordSecondStepRequestModel.ControlHash &&
                t.UserHash == remindPasswordSecondStepRequestModel.UserHash);

            if (user == null)
                return false;

            if (!user.IsChangingPassword || remindPasswordSecondStepRequestModel.ControlHash != user.ControlHash || remindPasswordSecondStepRequestModel.UserHash != user.UserHash)
                return false;

            var newPassword = _encryptor.GetHashPassword(remindPasswordSecondStepRequestModel.NewPassword, _applicationSettings.SaltCount,
                user.Id);

            user.IsChangingPassword = false;

            return await ChangePassword(user, newPassword);
        }

        public async Task<bool> RemindPasswordFirstStep(string loginOrEmail)
        {
            var user = await GetUserByLoginOrEmail(loginOrEmail);

            if (user == null) return false;

            if (user.Block) return false;

            user.IsChangingPassword = true;

            var dbResult = _context.Users.Update(user);

            if (dbResult.State != EntityState.Modified) return false;

            var emailResult = await _emailNotificationService.SendRemindPasswordEmail(user);

            if(emailResult)
                await _context.SaveChangesAsync();

            return emailResult;
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

        public async Task<User> GetUserByIdAndUserHash(int id, string userHash)
        {
            return await _context.Users.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ChangeEmailAddress(ChangeEmailAddressRequestModel changeEmailAddressRequestModel)
        {
            if (!changeEmailAddressRequestModel.EmailAddress.HasValidEmailAddress())
                return false;

            var user = await GetUserByIdAndUserHash(changeEmailAddressRequestModel.Id, changeEmailAddressRequestModel.UserHash);

            if (user == null)
                return false;

            var currentUserEmails = await
                _context.Users.AnyAsync(t => t.Email == changeEmailAddressRequestModel.EmailAddress);

            if (currentUserEmails)
                return false;

            var isArchival = await 
                _context.ArchivalEmailAddresses.AnyAsync(t => t.EmailAddress == changeEmailAddressRequestModel.EmailAddress);

            if (isArchival)
                return false;

            var addResult = await _context.ArchivalEmailAddresses.AddAsync(new ArchivalEmailAddress()
                {EmailAddress = user.Email, UserId = user.Id});

            if (addResult.State != EntityState.Added)
                return false;

            user.Email = changeEmailAddressRequestModel.EmailAddress;
            user.UserHash = user.GetHash();

            var updateResult = _context.Users.Update(user);

            if (updateResult.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ConfirmUserEmail(EmailConfirmRequestModel emailConfirmRequestModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(t =>
                t.ControlHash == emailConfirmRequestModel.ControlHash &&
                t.UserHash == emailConfirmRequestModel.UserHash);

            if (user == null)
                return false;

            user.EmailConfirmed = true;

            var updateResult = _context.Users.Update(user);

            if (updateResult.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
