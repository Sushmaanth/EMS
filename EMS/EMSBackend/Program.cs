
using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using Dtos.Validation;
using Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

            builder.Services.AddSwaggerGen();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
