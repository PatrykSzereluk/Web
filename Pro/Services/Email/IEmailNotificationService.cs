namespace Pro.Services.Email
{
    using System.Collections.Generic;
    using Models.Enums;
    using System.Threading.Tasks;
    using Pro.Models.Email;
    using Models.DB;

    public interface IEmailNotificationService
    {
        Task<bool> SendRegisterEmail(User user);
        Task<bool> SendRemindPasswordEmail(User user);
    }
}
