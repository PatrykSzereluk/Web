using Pro.Models.User;

namespace Pro.Services.ManageUser
{
    using Pro.Models.ManageUser;
    using System.Threading.Tasks;
    using Models.Ban;
    public interface IManageUserService
    {
        Task<bool> BanUser(BanUserRequestModel banUserRequestModel);
        Task<bool> UnblockUser(UnblockUserRequestModel unblockUserRequestModel);
        Task<bool> ChangeUserRole(ChangeUserRoleRequestModel changeUserRoleRequestModel);
        Task<bool> ForceChangeUserEmail(ChangeEmailAddressRequestModel changeEmailAddressRequestModel);
    }
}
