﻿namespace Pro.Controllers.Admin
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Ban;
    using Services.ManageUser;
    using Models.ManageUser;
    using Models.User;

    public class ManagerUserController : ApiControllerBase
    {
        private readonly IManageUserService _manageUserService;
        public ManagerUserController(IManageUserService manageUserService)
        {
            _manageUserService = manageUserService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route(nameof(BanUser))]
        public async Task<bool> BanUser(BanUserRequestModel banUserRequestModel)
        {
           return await _manageUserService.BanUser(banUserRequestModel);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route(nameof(UnblockUser))]
        public async Task<bool> UnblockUser(UnblockUserRequestModel unblockUserRequestModel)
        {
            return await _manageUserService.UnblockUser(unblockUserRequestModel);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route(nameof(ChangeUserRole))]
        public async Task<bool> ChangeUserRole(ChangeUserRoleRequestModel changeUserRoleRequestModel)
        {
            return await _manageUserService.ChangeUserRole(changeUserRoleRequestModel);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route(nameof(ForceChangeUserEmail))]
        public async Task<bool> ForceChangeUserEmail(ChangeEmailAddressRequestModel changeEmailAddressRequest)
        {
            return await _manageUserService.ForceChangeUserEmail(changeEmailAddressRequest);
        }
    }
}
