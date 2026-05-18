
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
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var employeeBuilder = modelBuilder.Entity<Employee>();

            var departmentBuilder = modelBuilder.Entity<Department>();

            var userBuilder = modelBuilder.Entity<User>();

            var roleBuilder = modelBuilder.Entity<Role>();


            //[Employee Table]

            //PK
            employeeBuilder.ToTable("Employee").HasKey(e => e.Id);

            //Id
            employeeBuilder.Property<int>(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            //Name - 100 char
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

            //[User]
            //pk
            userBuilder.ToTable("User").HasKey(u => u.Id);

            //Id
            userBuilder.Property<int>(u => u.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            //Email Id
            userBuilder.HasIndex(u => u.EmailId)
               .IsUnique();

            userBuilder.Property<string>(e => e.EmailId)
                .HasColumnType("varchar(255)");

            //One Role - Many User FK
            userBuilder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            //Is Active
            userBuilder.Property<bool>(e => e.IsActive)
                .HasColumnType("bit");

            //One Use One Employee FK Employee Id
            userBuilder.HasOne(e => e.Employee)
                .WithOne(u => u.User)
                .HasForeignKey<User>(u => u.EmployeeId);

            //[Role]
            roleBuilder.ToTable("Role").HasKey(r => r.Id);

            //Pk
            roleBuilder.Property<int>(r => r.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            //role Name
            roleBuilder.Property<string>(r=> r.RoleName)
               .HasColumnType("varchar(20)");
           
        }
    }
}
