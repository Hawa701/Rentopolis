using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Rentopolis.Utility
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RoleBasedDefaultRouteMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedDefaultRouteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var user = httpContext.User;

                if (user.IsInRole("Admin"))
                {
                    httpContext.Response.Redirect("/Admin/Home");
                    return;
                }
                else if (user.IsInRole("Manager"))
                {
                    httpContext.Response.Redirect("/Manager/Home");
                    return;
                }
                else if (user.IsInRole("Landlord"))
                {
                    httpContext.Response.Redirect("/Landlord/Home");
                    return;
                }
                else if (user.IsInRole("Tenant"))
                {
                    httpContext.Response.Redirect("/Tenant/Home");
                    return;
                }
            }

            // Continue to the next middleware if the user is not authenticated or doesn't match any specific role
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RoleBasedDefaultRouteMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleBasedDefaultRouteMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleBasedDefaultRouteMiddleware>();
        }
    }
}
