
using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using EMSAuthApi.Services;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EMSAuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IUserRepository<ActivateAccountDTO>,UserRepository>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options=>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],

                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]))


                    };
                });
            builder.Services.AddAuthorization();
            var connStr = builder.Configuration.GetConnectionString("employeeManagementDbConStr");

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr, config => config.MigrationsAssembly("EMSBackend")));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
