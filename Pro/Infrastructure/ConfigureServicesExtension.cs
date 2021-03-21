namespace Pro.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;
    using Services.Identity;
    using Security.Cryptography;
    using Services.Email;
    using Services.Claims;
    using Services.User;
    using Services.UserService;
    using Services.ManageUser;
    using Pro.Services.File;

    public static class ConfigureServicesExtension
    {
        public static IServiceCollection AddTransientCollection(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IIdentityService, IdentityServices>()
                .AddTransient<IClaimsService, ClaimsServices>()
                .AddTransient<IManageUserService, ManageUserService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IEncryptor, Encryptor>()
                .AddTransient<IEmailNotificationService, MailKitNotificationService>()
                .AddTransient<IFileService, FileService>();
            
            return serviceCollection;
        }
    }
}
