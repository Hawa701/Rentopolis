using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis;
using Rentopolis.Models.Data;
using Rentopolis.Repositories.Implementations;
using Rentopolis.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// for context class
builder.Services.AddDbContext<RentContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyConnection")
    )
);

//for interfaces
builder.Services.AddScoped<IAccountServices, AccountServices>();

// for identity and role
builder.Services.AddIdentity<AppUser, IdentityRole>()
    //.AddRoleManager<RoleManager<IdentityRole>>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RentContext>()
    .AddDefaultTokenProviders();

// for security? idk
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<RoleBasedDefaultRouteMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
