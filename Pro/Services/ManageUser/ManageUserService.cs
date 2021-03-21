using Pro.Models.Enums;
using Pro.Models.ManageUser;
using Pro.Models.User;

namespace Pro.Services.ManageUser
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Models.Ban;
    using Models.DB;
    using UserService;


    public class ManageUserService : IManageUserService
    {
        private readonly ProContext _context;
        private readonly IUserService _userService;

        public ManageUserService(ProContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<bool> BanUser(BanUserRequestModel banUserRequestModel)
        {
            var user = await _userService.GetUserById(banUserRequestModel.Id);

            if (user == null) 
                return false;

            user.Block = true;
            var updateResult = _context.Users.Update(user);

            if (updateResult.State != EntityState.Modified)
                return false;

            var newBan = new Ban()
            {
                BanReason = (byte) banUserRequestModel.BanReason,
                Description = banUserRequestModel.Description,
                UserId = user.Id,
                IsActive = true
            };

            var addedResult = await _context.Bans.AddAsync(newBan);

            if (addedResult.State != EntityState.Added)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnblockUser(UnblockUserRequestModel unblockUserRequestModel)
        {
            var lastBlockResult = await _context.Bans.FirstOrDefaultAsync(t => t.UserId == unblockUserRequestModel.Id);

            if (lastBlockResult == null)
                return false;

            var user = await _userService.GetUserById(unblockUserRequestModel.Id);

            if (user == null)
                return false;

            lastBlockResult.IsActive = false;

            var updateBanResult= _context.Bans.Update(lastBlockResult);

            if (updateBanResult.State != EntityState.Modified)
                return false;

            user.Block = false;

            var updateUserResult = _context.Users.Update(user);

            if (updateUserResult.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangeUserRole(ChangeUserRoleRequestModel changeUserRoleRequestModel)
        {
            if (changeUserRoleRequestModel.RoleType == RoleType.SuperAdmin)
                return false;

            var user = await _userService.GetUserById(changeUserRoleRequestModel.Id);

            if (user == null)
                return false;

            user.RoleType = (short)changeUserRoleRequestModel.RoleType;

            var updateResult = _context.Users.Update(user);

            if (updateResult.State != EntityState.Modified)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForceChangeUserEmail(ChangeEmailAddressRequestModel changeEmailAddressRequestModel)
        {
            var user = await _userService.GetUserById(changeEmailAddressRequestModel.Id);

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

            user.Email = changeEmailAddressRequestModel.EmailAddress;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}