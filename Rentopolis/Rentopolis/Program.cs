using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Repositories.Implementations;
using Rentopolis.Repositories.Interfaces;
using Rentopolis.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// for context class
builder.Services.AddDbContext<RentContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyConnection")
    )
);

// for identity and role
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RentContext>()
    .AddDefaultTokenProviders();

// for interfaces
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddScoped<IManagerServices, ManagerServices>();
builder.Services.AddScoped<ILandlordServices, LandlordServices>();
builder.Services.AddScoped<IPropertyServices, PropertyServices>();

// for unauthenticated user redirection
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

// for unauthorized user redirection
builder.Services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Account/AccessDenied");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// for 404 error
app.UseStatusCodePagesWithRedirects("/Home/NotFound");

app.UseHttpsRedirection();
app.UseStaticFiles();

// custom middleware for default routing
//app.UseMiddleware<RoleBasedDefaultRouteMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seeding
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Manager", "Landlord", "Tenant" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();
