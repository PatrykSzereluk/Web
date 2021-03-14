﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro.Models.User;
using Pro.Services.UserService;

namespace Pro.Controllers
{
    [Authorize]
    public class UserController : ApiControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPatch]
        [Route(nameof(ChangePasswordWithOldPassword))]
        public async Task<bool> ChangePasswordWithOldPassword(ChangePasswordRequestModel changePasswordRequestModel)
        {
            return await _userService.ChangePasswordWithOldPassword(changePasswordRequestModel);
        }

        [HttpPost]
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

        [HttpPost]
        [Route(nameof(ChangeEmailAddress))]
        public async Task<bool> ChangeEmailAddress(ChangeEmailAddressRequestModel changeEmailAddressRequestModel)
        {
            return await _userService.ChangeEmailAddress(changeEmailAddressRequestModel);
        }
    }
}
