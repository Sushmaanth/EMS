
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options): base(options)
        {
            
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var employeeBuilder = modelBuilder.Entity<Employee>();

            var departmentBuilder = modelBuilder.Entity<Department>();

            //[Employee Table]

            //PK
            employeeBuilder.ToTable("Employee").HasKey(e => e.Id);

            //Id
            employeeBuilder.Property<int>(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            //Name
            employeeBuilder.Property<string>(e => e.Name)
                .HasColumnType("varchar(200)");

            //EmailID - Unique Constraint
            employeeBuilder.HasIndex(e => e.EmailId)
                .IsUnique();

            //EmailID
            employeeBuilder.Property<string>(e => e.EmailId)
                .HasColumnType("varchar(255)");

            //Mobile No
            employeeBuilder.Property<long>(e => e.Mobile)
                .HasColumnType("bigint");

            // Gender
            employeeBuilder.Property<string>(e => e.Gender)
                .HasColumnType("varchar(20)");

            // Date Of Birth
            employeeBuilder.Property(e => e.DateOfBirth)
                .HasColumnType("date");

            // Salary
            employeeBuilder.Property<decimal>(e => e.Salary)
                .HasColumnType("decimal(18,2)");

            // Date Of Joining
            employeeBuilder.Property(e => e.DateOfJoining)
                .HasColumnType("date");

            //Fk - Department Id
            employeeBuilder.HasOne(d => d.Department)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.DepartmentId);

            //[Department] - PK
            departmentBuilder.ToTable("Department").HasKey(d => d.Id);

            //Id
            departmentBuilder.Property<int>(d => d.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            //Name
            departmentBuilder.Property<string>(d => d.DepartmentName)
                .HasColumnType("varchar(200)");
        }
    }
}
