
using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using Dtos.Validation;
using Entities.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace EMSBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            var connStr = builder.Configuration.GetConnectionString("employeeManagementDbConStr");

            builder.Services.AddDbContext<AppDbContext>(opt=> opt.UseSqlServer(connStr, config=> config.MigrationsAssembly("EMSBackend")));
            builder.Services.AddScoped<IRepository<EmployeeDto>, EmployeeRepository>();
            builder.Services.AddScoped<IEmployeeRepository<EmployeeDto>, EmployeeRepository>();
            builder.Services.AddScoped<EmployeeValidator>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
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

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",

                        Type =
                            Microsoft.OpenApi.Models.SecuritySchemeType.Http,

                        Scheme = "bearer",

                        BearerFormat = "JWT",

                        In =
                            Microsoft.OpenApi.Models.ParameterLocation.Header,

                        Description =
                            "Enter JWT Token"
                    });

                options.AddSecurityRequirement(
                    new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType
                                    .SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
                    });
            });
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
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
