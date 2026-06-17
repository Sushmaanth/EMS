
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.Implementation;
using EMSFrontend.GlobalException;
using EMSFrontend.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace EMSFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string baseUrl = builder.Configuration["RequestUrls:EmployeeRequestUrl"]!;

            builder.Services.AddHttpClient<IRequest, EmployeeApiRequest>()
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromMinutes(5);
                });

            string validationUrl = builder.Configuration["RequestUrls:ValidationRequestUrl"]!;
            builder.Services.AddHttpClient<IValidationRequest, ValidationApiRequest>()
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress =
                        new Uri(builder.Configuration["RequestUrls:ValidationRequestUrl"]!);
                });

            string leaveRequestUrl = builder.Configuration["RequestUrls:LeaveRequestUrl"]!;

            builder.Services.AddHttpClient<ILeaveApiRequest, LeaveApiRequest>()
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(leaveRequestUrl);
                    client.Timeout = TimeSpan.FromMinutes(5);
                });

            string authUrl = builder.Configuration["RequestUrls:AuthRequestUrl"]!;

            builder.Services.AddHttpClient<IAuthRequest, AuthApiRequest>()
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(authUrl);
                    client.Timeout = TimeSpan.FromMinutes(5);
                });

            builder.Services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                builder.Configuration.Bind("AzureAd", options);

                options.Prompt = "select_account";
            });

            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);

                options.Cookie.HttpOnly = true;

                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<IApiRequestHelper, ApiRequestHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Admin/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapStaticAssets();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Admin}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
