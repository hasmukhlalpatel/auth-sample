using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

//Kestrel// IISServer/IIS express
builder.Services
    .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();  // Enables Windows Authentication

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
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

app.MapGet("/", () => "Hello World!");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
