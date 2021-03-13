
using Pro.Services.UserService;

namespace Pro.Services.Identity
{
    using Microsoft.EntityFrameworkCore;
    using Models.DB;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using System;
    using Models.Enums;
    using Security.Cryptography;
    using Models.Identity;
    using Email;
    using Helpers;

    public class IdentityServices : IIdentityService
    {
        private readonly ProContext _context;
        private readonly IEncryptor _encryptor;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IUserService _userService;

        public IdentityServices(ProContext context, IEncryptor encryptor, IOptions<ApplicationSettings> applicationSettings, IEmailNotificationService emailNotificationService, IUserService userService)
        {
            _context = context;
            _encryptor = encryptor;
            _emailNotificationService = emailNotificationService;
            _userService = userService;
            _applicationSettings = applicationSettings.Value;
        }

        public async Task<LoginResponseModel> Login(LoginRequestModel loginRequestModel)
        {
            User user = await _userService.GetUserByLoginOrEmail(loginRequestModel.LoginOrEmail, true);

            if (user == null) return new LoginResponseModel() { Id = -1, StatusCode = ResponseStatusCode.Failed };

            var fullPassword = _encryptor.GetHashPassword(loginRequestModel.Password, _applicationSettings.SaltCount, user.Id);

            if (user.Password.Equals(fullPassword))
            {
                if (user.CheckPassword && user.LastPasswordChanged.HasValue)
                {
                    var lastPasswordChanged = user.LastPasswordChanged.Value;

                    if (lastPasswordChanged.AddDays(90) >= DateTime.Now)
                    {
                        return new LoginResponseModel() { Id = user.Id, StatusCode = ResponseStatusCode.PendingForChangePassword };
                    }
                }

                return new LoginResponseModel() { StatusCode = ResponseStatusCode.Success, Id = user.Id };
            }

            return new LoginResponseModel() { StatusCode = ResponseStatusCode.Failed };
        }

        public async Task<RegisterResponseModel> RegisterUser(RegisterRequestModel registerRequestModel)
        {
            //check nip

            if (!registerRequestModel.Login.HasOnlyDigitOrLetter()
               || !registerRequestModel.Email.HasValidEmailAddress()
               || !registerRequestModel.Password.HasValidCharacterPassword())
                return new RegisterResponseModel() { StatusCode = ResponseStatusCode.Failed };

            if (await _context.Users.AnyAsync(t => t.Login == registerRequestModel.Login || t.Email == registerRequestModel.Email))
                return new RegisterResponseModel() { StatusCode = ResponseStatusCode.Failed };

            var newUser = new User()
            {
                Login = registerRequestModel.Login,
                Email = registerRequestModel.Email,
                Password = string.Empty,
                ControlHash = string.Empty,
                UserHash = string.Empty,
                CheckPassword = false,
                LastPasswordChanged = DateTime.Now,
                UserTypeId = 1,
                IsChangingPassword = false
            };

            var insertResult = await _context.Users.AddAsync(
                newUser
                );

            if (insertResult.State != EntityState.Added)
                return new RegisterResponseModel() { StatusCode = ResponseStatusCode.Failed, Id = -1 };

            await _context.SaveChangesAsync();

            newUser.Password = _encryptor.GetHashPassword(registerRequestModel.Password, _applicationSettings.SaltCount, insertResult.Entity.Id);

            newUser.UserHash = newUser.GetHash();

            newUser.ControlHash = newUser.GetHash();

            var updateResult = _context.Users.Update(newUser);

            if (updateResult.State != EntityState.Modified)
                _context.Users.Remove(newUser);
            
            await _context.SaveChangesAsync();

            var emailResult = await _emailNotificationService.SendRegisterEmail(newUser);

            if (!emailResult)
            {
                _context.Users.Remove(newUser);
                await _context.SaveChangesAsync();
                return new RegisterResponseModel() { StatusCode = ResponseStatusCode.Failed };
            }

            return new RegisterResponseModel() { StatusCode = ResponseStatusCode.Success };
        }
    }
}
