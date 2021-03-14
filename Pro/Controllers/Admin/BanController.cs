namespace Pro.Controllers.Admin
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Ban;
    using Services.Ban;

    [Authorize(Roles = "Admin")]
    public class BanController : ApiControllerBase
    {
        private readonly IBanService _banService;
        public BanController(IBanService banService)
        {
            _banService = banService;
        }

        [HttpPost]
        [Route(nameof(BanUser))]
        public async Task<bool> BanUser(BanUserRequestModel banUserRequestModel)
        {
           return await _banService.BanUser(banUserRequestModel);
        }
    }
}
