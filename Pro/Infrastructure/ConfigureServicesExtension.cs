
using Pro.Services.UserService;

namespace Pro.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;
    using Services.Identity;
    using Security.Cryptography;
    using Services.Email;

    public static class ConfigureServicesExtension
    {
        public static IServiceCollection AddTransientCollection(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IIdentityService, IdentityServices>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IEncryptor, Encryptor>()
                .AddTransient<IEmailNotificationService, MailKitNotificationService>();
            
            return serviceCollection;
        }
    }
}
