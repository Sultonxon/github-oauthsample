using Microsoft.AspNetCore.Identity;
using ServerApp.Services;

namespace ServerApp.Extensions;

public static class Extensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static async Task SeedData(this IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.CreateScope()
            .ServiceProvider.GetService<UserManager<IdentityUser>>();

        var user = new IdentityUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "sultonxon",
            Email = "qudratovsultonxon20011124@gmail.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, "Secret123$");

    }
    
}