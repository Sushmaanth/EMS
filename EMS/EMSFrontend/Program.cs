using Dtos.Validation;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Api.Implementation;

namespace EMSFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string baseUrl = builder.Configuration.GetRequiredSection("RequestUrls:EmployeeRequestUrl").Value;

            builder.Services.AddHttpClient<IRequest, EmployeeApiRequest>(config => config.BaseAddress = new Uri(baseUrl));

            string authUrl = builder.Configuration.GetRequiredSection("RequestUrls:AuthRequestUrl").Value;

            builder.Services.AddHttpClient<IAuthRequest,AuthApiRequest>(config =>config.BaseAddress = new Uri(authUrl));

            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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
