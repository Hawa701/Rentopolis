using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Rentopolis.Models.Data;
using System.Threading.Tasks;


namespace Rentopolis.Utility
{
    public class RoleBasedDefaultRouteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<AppUser> _userManager;

        public RoleBasedDefaultRouteMiddleware(RequestDelegate next, UserManager<AppUser> userManager)
        {
            _next = next;
            _userManager = userManager;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            if (httpContext.User.Identity.IsAuthenticated)
            {
                //var user = httpContext.User;
                //var userManager = _userManager;
                var user = await _userManager.GetUserAsync(httpContext.User);

                //if (user.IsInRole("Admin"))
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    httpContext.Response.Redirect("/Admin/Home");
                    return;
                }
                else if (await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    httpContext.Response.Redirect("/Manager/Home");
                    return;
                }
                else if (await _userManager.IsInRoleAsync(user, "Landlord"))
                {
                    httpContext.Response.Redirect("/Landlord/Home");
                    return;
                }
                else if (await _userManager.IsInRoleAsync(user, "Tenant"))
                {
                    httpContext.Response.Redirect("/Property/Listings");
                    return;
                }
            }

            // Continue to the next middleware if the user is not authenticated or doesn't match any specific role
            await _next(httpContext);
        }
    }

    public static class RoleBasedDefaultRouteMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleBasedDefaultRouteMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleBasedDefaultRouteMiddleware>();
        }
    }
}
