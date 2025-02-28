using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication;

namespace WinAuth.WebApp.Extensions
{
    public static class WinAuthorizationAppBuilderExtensions
    {
        public static IApplicationBuilder UseChallengeAuthentication(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    Console.WriteLine("User is not authenticated. Sending challenge.");
                    await context.ChallengeAsync(NegotiateDefaults.AuthenticationScheme);
                }
                else
                {
                    Console.WriteLine($"User: {context.User.Identity.Name}");
                    await next();
                }
            });
        }
    }
}
