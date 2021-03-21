using Microsoft.AspNetCore.Http;
using Pro.Services.File;

namespace Pro.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.User;
    using Services.Claims;
    using Services.UserService;

    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;
        private readonly IFileService _fileService;

        public UserController(IUserService userService, IClaimsService claimsService, IFileService fileService)
        {
            _userService = userService;
            _claimsService = claimsService;
            _fileService = fileService;
        }

        [Authorize]
        [HttpPatch]
        [Route(nameof(ChangePasswordWithOldPassword))]
        public async Task<bool> ChangePasswordWithOldPassword(ChangePasswordRequestModel changePasswordRequestModel)
        {
            if (!_claimsService.CheckUserId(changePasswordRequestModel.Id))
                return false;

            return await _userService.ChangePasswordWithOldPassword(changePasswordRequestModel);
        }

        [HttpGet]
        [Route(nameof(RemindPasswordFirstStep))]
        public async Task<bool> RemindPasswordFirstStep(RemindPasswordFirstStepRequestModel remindPasswordFirstStepRequestModel)
        {
            return await _userService.RemindPasswordFirstStep(remindPasswordFirstStepRequestModel.LoginOrEmail);
        }

        [HttpPost]
        [Route(nameof(RemindPasswordSecondStep))]
        public async Task<bool> RemindPasswordSecondStep(RemindPasswordSecondStepRequestModel remindPasswordSecondStepRequestModel)
        {
            return await _userService.RemindPasswordSecondStep(remindPasswordSecondStepRequestModel);
        }

        [Authorize]
        [HttpPost]
        [Route(nameof(ChangeEmailAddress))]
        public async Task<bool> ChangeEmailAddress(ChangeEmailAddressRequestModel changeEmailAddressRequestModel)
        {
            if (!_claimsService.CheckUserId(changeEmailAddressRequestModel.Id))
                return false;

            return await _userService.ChangeEmailAddress(changeEmailAddressRequestModel);
        }

        [HttpPost]
        [Route(nameof(ConfirmEmail))]
        public async Task<bool> ConfirmEmail(EmailConfirmRequestModel emailConfirmRequestModel)
        {
            return await _userService.ConfirmUserEmail(emailConfirmRequestModel);
        }

        [Authorize]
        [HttpPost]
        [Route(nameof(UploadAvatar))]
        public async Task<bool> UploadAvatar(IFormFile file)
        {
            var userId = _claimsService.GetUserId();
            if (userId == string.Empty)
                return false;

            return await _fileService.UploadAvatar(file, userId);
        }
    }
}
