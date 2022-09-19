using System.Configuration;
using BankAPP.Mail;
using BankAPP.SignalRServer;
using BankModel.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.Options;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2VVhiQlFaclxJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1hX39XdHBVQWNcU0E=");
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/login";
//        options.AccessDeniedPath = "/loginAdmin";
//        options.Events = new CookieAuthenticationEvents()
//        {
//            OnSigningIn = async context =>
//            {
//                await Task.CompletedTask;
//            },
//            OnSignedIn = async context =>
//            {
//                await Task.CompletedTask;
//            },
//            OnValidatePrincipal = async context =>
//            {
//                await Task.CompletedTask;
//            }
//        };
//    });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "UserAuth";
    })
        .AddCookie("UserAuth", options =>
        {
            options.LoginPath = "/Login";
            options.AccessDeniedPath = "/AccessDenied";
            options.Events = new CookieAuthenticationEvents()
            {
                OnSigningIn = async context =>
                {
                    await Task.CompletedTask;
                },
                OnSignedIn = async context =>
                {
                    await Task.CompletedTask;
                },
                OnValidatePrincipal = async context =>
                {
                    await Task.CompletedTask;
                }
            };

        })
        .AddCookie("AdminAuth", options =>
        {
            options.LoginPath = "/loginAdmin";
            options.AccessDeniedPath = "/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        });
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

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.MapHub<Notification>("/notificationServer");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.Run();
