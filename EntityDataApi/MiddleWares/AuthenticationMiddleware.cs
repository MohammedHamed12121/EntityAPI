using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityDataApi.MiddleWares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/Authenticate"))
            {

                if (!context.User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("User is not authenticated. Authentication is required to access this endpoint.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers.Add("X-Register-Required", "true");
                    await context.Response.WriteAsync("Authentication is required to access this endpoint. Please authenticate.");
                    return;
                }

            }
            await _next(context);
        }
    }
}