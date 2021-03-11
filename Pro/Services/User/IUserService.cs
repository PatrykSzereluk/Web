namespace Pro.Services.UserService
{
    using System.Threading.Tasks;
    using Models.User;
    using Pro.Models.DB;

    public interface IUserService
    {
        Task<bool> ChangePasswordWithOldPassword(ChangePasswordRequestModel changePasswordRequestModel);
        Task<User> GetUserByLoginOrEmail(string loginOrEmail, bool isLogin = false);
        Task<bool> RemindPasswordFirstStep(string loginOrEmail);
        Task<bool> RemindPasswordSecondStep(RemindPasswordSecondStepRequestModel remindPasswordSecondStepRequestModel);
        Task<User> GetUserById(int id);
    }
}
