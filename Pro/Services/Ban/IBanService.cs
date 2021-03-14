namespace Pro.Services.Ban
{
    using System.Threading.Tasks;
    using Pro.Models.Ban;
    public interface IBanService
    {
        Task<bool> BanUser(BanUserRequestModel banUserRequestModel);
    }
}
