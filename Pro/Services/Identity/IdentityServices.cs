
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
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

            if (!user.EmailConfirmed)
                return new LoginResponseModel() {Id = -1, StatusCode = ResponseStatusCode.PendingForConfirmEmail};

            var fullPassword = _encryptor.GetHashPassword(loginRequestModel.Password, _applicationSettings.SaltCount, user.Id);

            if (!user.Password.Equals(fullPassword))
                return new LoginResponseModel() { StatusCode = ResponseStatusCode.Failed };

            ResponseStatusCode statusCode = ResponseStatusCode.Success;

            if (user.CheckPassword && user.LastPasswordChanged.HasValue)
            {
                var lastPasswordChanged = user.LastPasswordChanged.Value;

                if (lastPasswordChanged.AddDays(90) >= DateTime.Now)
                {
                    statusCode = ResponseStatusCode.PendingForChangePassword;
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_applicationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, user.Id.ToString()),
                    new(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptToken = tokenHandler.WriteToken(token);

            return new LoginResponseModel() { StatusCode = statusCode, Id = user.Id, Token = encryptToken};
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
                IsChangingPassword = false,
                EmailConfirmed = false
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
