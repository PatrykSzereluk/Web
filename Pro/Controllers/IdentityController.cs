
using System.Text.RegularExpressions;

namespace Pro.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.Identity;
    using Services.Identity;
    using System.Threading.Tasks;
  
    public class IdentityController : ApiControllerBase
    {
        private readonly IIdentityService _identityService;
        
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<RegisterResponseModel> Register(RegisterRequestModel registerRequestModel)
        {
            return await _identityService.RegisterUser(registerRequestModel);
        }

        [HttpGet]
        [Route(nameof(Login))]
        public async Task<LoginResponseModel> Login(LoginRequestModel loginRequestModel)
        {
            return await _identityService.Login(loginRequestModel);
        }
    }
}
