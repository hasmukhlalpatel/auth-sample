using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Security.Principal;
using WinAuth.WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Kestrel// IISServer/IIS express
builder.Services
    .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();  // Enables Windows Authentication

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminsOnly", policy => policy.RequireRole("HASU-DEV\\Administrators")); // Change DOMAIN
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("EveryOne", policy => policy.RequireRole("EveryOne"));

    // Default policy for unauthenticated users
    //options.FallbackPolicy = options.DefaultPolicy;
});

/* Kerberos authentication and role-based access control (RBAC)
 * builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate(options =>
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            options.EnableLdap("contoso.com");
            //options.EnableLdap(settings =>
            //    {
            //        settings.Domain = "contoso.com";
            //        settings.MachineAccountName = "machineName";
            //        settings.MachineAccountPassword =
            //                          builder.Configuration["Password"];
            //    });
        }
    });
 */

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


//To chalnge manual Authentication/ ask or username and password
//app.UseChallengeAuthentication();

app.UseAuthentication();
app.UseAuthorization();

//app.UseWhen(context => !context.Request.Path.StartsWithSegments("/public"), appBuilder =>
//{
//    appBuilder.UseAuthentication();
//    appBuilder.UseAuthorization();
//});

// Public endpoint
app.MapGet("/", (HttpContext context) => $"Hello World! User: {context?.User?.Identity?.Name}");
app.MapGet("/public", (HttpContext context) => $"Hello World! User: {context?.User?.Identity?.Name}");

// Secured endpoint (authenticated users)
app.MapGet("/secure", (HttpContext context) =>
{
    return $"Authenticated as: {context.User.Identity?.Name}";
}).RequireAuthorization();

// Secured endpoint with role
app.MapGet("/admin", (HttpContext context) =>
{
    return $"Admin access granted to: {context.User.Identity?.Name}";
}).RequireAuthorization("AdminsOnly");

app.MapGet("/RequireAdmin", (HttpContext context) =>
{
    return $"Admin access granted to: {context.User.Identity?.Name}";
}).RequireAuthorization("RequireAdmin");

app.MapGet("/EveryOne", (HttpContext context) =>
{
    return $"EveryOne access granted to: {context.User.Identity?.Name}";
}).RequireAuthorization("EveryOne");

app.MapGet("/groups", (HttpContext context) =>
{
    var user = context.User;

    var groups = user.Claims
        .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();

    return Results.Ok(new
    {
        User = user.Identity?.Name,
        Groups = groups
    });
}).RequireAuthorization();

app.MapGet("/groupsid", (HttpContext context) =>
{
    var user = context.User;

    var groupNames = user.Claims
        .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid")
        .Select(c => {
            try
            {
                var sid = new SecurityIdentifier(c.Value);
                return sid.Translate(typeof(NTAccount)).ToString(); // DOMAIN\GroupName
            }
            catch
            {
                return $"[Unresolvable SID: {c.Value}]";
            }
        })
        .ToList();

    return Results.Ok(new
    {
        User = user.Identity?.Name,
        Groups = groupNames
    });
}).RequireAuthorization();

app.Run();
