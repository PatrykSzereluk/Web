namespace Pro.Services.Identity
{
    using Models.DB;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pro.Models.Identity;

    public interface IIdentityService
    {
        Task<RegisterResponseModel> RegisterUser(RegisterRequestModel registerRequestModel);
        Task<LoginResponseModel> Login(LoginRequestModel loginRequestModel);
        
    }
}