using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pro.Models.User;
using Pro.Services.Claims;
using Pro.Services.UserService;

namespace Pro.Controllers
{
    
    public class UserController : ApiControllerBase
    {

        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;

        public UserController(IUserService userService, IClaimsService claimsService)
        {
            _userService = userService;
            _claimsService = claimsService;
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
    }
}
