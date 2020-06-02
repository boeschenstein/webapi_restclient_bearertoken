# WebAPI + REST client + Bearer token

## Optional, rarely needed

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


Handler:

```c#
    public class CustomAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            //var authFilterCtx = (Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)context.Resource;
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
