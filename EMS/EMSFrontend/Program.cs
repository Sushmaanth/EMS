using Dtos.Validation;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.Implementation;
using EMSFrontend.GlobalException;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

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

            string authUrl = builder.Configuration["RequestUrls:AuthRequestUrl"]!;

            builder.Services.AddHttpClient<IAuthRequest, AuthApiRequest>()
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(authUrl);
                    client.Timeout = TimeSpan.FromMinutes(5);
                });

            //Microsoft OAuth
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    MicrosoftAccountDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddMicrosoftAccount(options =>
            {
                options.ClientId =
                    builder.Configuration["Authentication:Microsoft:ClientId"];

                options.ClientSecret =
                    builder.Configuration["Authentication:Microsoft:ClientSecret"];

                options.AuthorizationEndpoint += "?prompt=select_account";
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
            app.MapStaticAssets();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
