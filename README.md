# WebAPI + REST client + Bearer token

## Access Token in Controller

```c#
string scheme = "Bearer";
string result1 = await HttpContext.GetTokenAsync(scheme, "access_token"); // works, but it is just the token as string
string result2 = await HttpContext.GetTokenAsync("access_token"); // works, but it is just the token as string

// AuthenticateResult result3a = await HttpContext.AuthenticateAsync(); // works also
AuthenticateResult result = await HttpContext.AuthenticateAsync(scheme);
ClaimsPrincipal principal = result.Principal;
IEnumerable<Claim> claims = principal.Claims;
foreach (var item in claims)
{
    // Console.WriteLine($"Role: {item.Type}:{item.Value}");
    if (item.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || item.Type == "role")
        Console.WriteLine($"Role: {item.Value}");
    if (item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || item.Type == "sub")
        Console.WriteLine($"Subject: {item.Value}");
}

```

## Access Token in StartUp (rarely used)

```c#
OnTokenValidated = contex =>
{
    Console.WriteLine($"---- OnTokenValidated Start ----");
    Console.WriteLine($"Issuer: {contex.SecurityToken.Issuer}");
    JwtSecurityToken jwt = (JwtSecurityToken)contex.SecurityToken;
    foreach (Claim claim in jwt.Claims)
    {
        //Console.WriteLine($"Claim: {claim.Type}={claim.Value}");
        if (claim.Type == "role")
        {
            Console.WriteLine($"Role:{claim.Value}");
        }
    }
    Console.WriteLine($"Subject: {jwt.Subject}");
    foreach (string audience in jwt.Audiences)
    {
        Console.WriteLine($"Audience:{audience}");
    }
    Console.WriteLine($"---- OnTokenValidated End ----");

    return Task.CompletedTask;
},
```

## Optional, rarely needed

### CustomAuthorizationHandler
Register handler

```c#
public void ConfigureServices(IServiceCollection services)
{
    ...

    // CustomAuthorizationHandler instead of Authorize attribute
    services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();

    services.AddControllers();
}
```

Handler ()

```c#
    public class CustomAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            //var authFilterCtx = (Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)context.Resource; // todo: cast not working...
            //string authHeader = authFilterCtx.HttpContext.Request.Headers["Authorization"];
            //if (authHeader != null && authHeader.Contains("Bearer"))
            //{
            //    var token = authHeader.Replace("Bearer", "");
            //    // Now token can be used for further authorization
            //}

            // throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
```

## HttpClient

### Typed Client

Typed Client: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#typed-clients>

```c#
// How to register HttpClient using a Service and Interface: https://stackoverflow.com/questions/56091875/ihttpclientfactory-in-asp-net-core-typed-clients
services.AddHttpClient<ICheckPermissionClient, CheckPermissionClient>(c =>
{
    c.BaseAddress = new Uri("https://i-pfw.corproot.net");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("User-Agent", "SMMS-PFW-Client");
});
services.AddTransient<ICheckPermission, CheckPermission>();
```

### Generate API using Swagger and NSwag

- Get swagger file from REST Api
- Install NSWag UI
- Generate Client, using Swagger File
    - name: CheckPermissionClient
    - generate an interface: ICheckPermissionClient
- Register HttpClient (code above)
- Create a service:

```c#
public class CheckPermission : ICheckPermission
{
    private readonly ICheckPermissionClient client;

    public CheckPermission(ICheckPermissionClient client)
    {
        this.client = client;
    }
    public async Task<bool> HavePermissionAsync(string myArguments)
    {
        var ret = await this.client.CheckPermissionsAsync(myArguments);
        return ret;
    }
}
```

# Information

- <https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests>
