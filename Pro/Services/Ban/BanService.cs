namespace Pro.Services.Ban
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Pro.Models.Ban;
    using Models.DB;
    using UserService;

    public class BanService : IBanService
    {
        private readonly ProContext _context;
        private readonly IUserService _userService;

        public BanService(ProContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<bool> BanUser(BanUserRequestModel banUserRequestModel)
        {
            var user = await _userService.GetUserById(banUserRequestModel.Id);

            if (user == null) return false;

            user.Block = true;

            var updateResult = _context.Users.Update(user);

            if (updateResult.State != EntityState.Modified)
                return false;

            var newBan = new Ban()
            {
                BanReason = (byte) banUserRequestModel.BanReason,
                Description = banUserRequestModel.Description,
                UserId = user.Id
            };

            var addedResult = await _context.Bans.AddAsync(newBan);

            if (addedResult.State != EntityState.Added)
                return false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
