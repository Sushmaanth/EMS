
using Azure.Storage.Blobs;
using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using Dtos.Validation;
using Dtos.Validation.Abstraction;
using Dtos.Validation.Implementation;
using EMSBackend.Mapper;
using EMSBackend.Middleware;
using EMSBackend.Service.Abstraction;
using EMSBackend.Service.Implementation;
using Entities.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace EMSBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            var connStr = builder.Configuration.GetConnectionString("employeeManagementDbConStr");

            //builder.Services.AddDbContext<AppDbContext>(opt=> opt.UseSqlServer(connStr, config=> config.MigrationsAssembly("EMSBackend")));
            builder.Services.AddDbContextPool<AppDbContext>(opt => opt.UseSqlServer(connStr, config => config.MigrationsAssembly("EMSBackend")), poolSize: 32);


            builder.Services.AddAutoMapper(config => config.AddProfile<EmployeeMappingProfile>());
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IEmployeeService,EmployeeService>();
            builder.Services.AddScoped<IEmployeeValidation, EmployeeValidation>();
            builder.Services.AddScoped<IBlobService, BlobService>();
            builder.Services.AddScoped<IEmployeeDuplicateUploadValidator, EmployeeDuplicateUploadValidator>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Configuration.AddUserSecrets<Program>();

            builder.Services.AddSingleton(x =>new BlobServiceClient( builder.Configuration["AzureBlobStorage:ConnectionString"]));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,

                       ClockSkew = TimeSpan.Zero,

                       ValidIssuer = builder.Configuration["Jwt:Issuer"],

                       ValidAudience = builder.Configuration["Jwt:Audience"],

                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                           builder.Configuration["Jwt:Key"]))

                   };

                   options.Events = new JwtBearerEvents
                   {
                       OnChallenge = async context =>
                       {
                           context.HandleResponse();

                           context.Response.StatusCode = 401;

                           context.Response.ContentType = "application/json";

                           await context.Response.WriteAsJsonAsync(new
                           {
                               title = "Unauthorized",
                               status = 401
                           });
                       }
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

            // Warm up the database connection during application startup// to reduce first-request/login latency.using (var scope = app.Services.CreateScope())

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.CanConnectAsync();
                Console.WriteLine("EMS Backend Database warm-up completed.");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            { 
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
