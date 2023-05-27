using IdentityServer;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel()
    .UseUrls(builder.Configuration["KestrelUrls:https"], builder.Configuration["KestrelUrls:http"]);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("identity")));

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
    {
        options.UserInteraction.LoginUrl = "/auth/login";
        options.UserInteraction.LogoutUrl = "/auth/logout";
        options.UserInteraction.ErrorUrl = "/auth/error";
    })
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddInMemoryApiResources(Config.GetApiResources())
    .AddInMemoryClients(Config.GetClients(builder.Configuration))
    .AddAspNetIdentity<User>();

builder.Services.AddCors();

builder.Services.Configure<CookiePolicyOptions>(o =>
{
    o.OnAppendCookie = cookieContext =>
    {
        cookieContext.CookieOptions.SameSite = SameSiteMode.None;
        cookieContext.CookieOptions.Secure = true;
    };

    o.OnDeleteCookie = cookieContext =>
    {
        cookieContext.CookieOptions.SameSite = SameSiteMode.None;
        cookieContext.CookieOptions.Secure = true;
    };
});

var app = builder.Build();

app.EnsureSeedData();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All,
});

app.UseRouting();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["Cors"]));

app.UseIdentityServer();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});


app.Run();
