# auth-sample

This is a sample project to demonstrate how to use the `auth` package.

## usefull information
[About Windows authentication in Dotnet](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-9.0&tabs=visual-studio)

## How to use
1. Clone this repository
2. Open the solution in Visual Studio

## How to run
1. Press `F5` to run the project

## How to test
1. Open a browser
1. Navigate to `https://localhost:5001/WeatherForecast`

## Nuget packages information
### [Kestrel](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-9.0)
The [Microsoft.AspNetCore.Authentication.Negotiate](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.Negotiate) NuGet package can be used with Kestrel to support Windows Authentication using Negotiate and Kerberos on Windows, Linux, and macOS.

### [Windows environment configuration](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-9.0&tabs=visual-studio#windows-environment-configuration)
```cs
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages();

var app = builder.Build();
```

### IIS/IIS express
```cs   
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
```

#### launchSettings.json
```json
"iisSettings": {
    "windowsAuthentication": true,
    "anonymousAuthentication": false,
    "iisExpress": {
        "applicationUrl": "http://localhost:52171/",
        "sslPort": 44308
    }
}
```


### [Kerberos authentication and role-based access control (RBAC)](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/windowsauth?view=aspnetcore-9.0&tabs=visual-studio#kerberos-authentication-and-role-based-access-control-rbac)
```cs
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate(options =>
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            options.EnableLdap("contoso.com");
        }
    });
```

Some configurations may require specific credentials to query the LDAP domain. The credentials can be specified in the following highlighted options:
```cs
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate(options =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                options.EnableLdap(settings =>
                {
                    settings.Domain = "contoso.com";
                    settings.MachineAccountName = "machineName";
                    settings.MachineAccountPassword =
                                      builder.Configuration["Password"];
                });
            }
        });

builder.Services.AddRazorPages();
```