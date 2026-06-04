
using Microsoft.EntityFrameworkCore;

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
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var employeeBuilder = modelBuilder.Entity<Employee>();

            var departmentBuilder = modelBuilder.Entity<Department>();

            var userBuilder = modelBuilder.Entity<User>();

            var roleBuilder = modelBuilder.Entity<Role>();

            var employeeDocumentBuilder = modelBuilder.Entity<EmployeeDocument>();

            var documentCategoryBuilder = modelBuilder.Entity<DocumentCategory>();

            var documentTypeBuilder = modelBuilder.Entity<DocumentType>();

            //[Employee Table]

            //PK
            employeeBuilder.ToTable("Employee").HasKey(e => e.Id);

            //Id
            employeeBuilder.Property<int>(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            //Employee Code
            employeeBuilder.Property(e => e.EmployeeCode)
                           .HasColumnType("varchar(20)");

            employeeBuilder.HasIndex(e => e.EmployeeCode)
                .IsUnique();

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

            //[Employee Documents]
            //pk
            employeeDocumentBuilder.ToTable("EmployeeDocument").HasKey(e => e.Id);

            //id
            employeeDocumentBuilder.Property<int>(ed => ed.Id)
               .ValueGeneratedOnAdd()
               .UseIdentityColumn(1, 1);

            //OG file name
            employeeDocumentBuilder.Property<string>(ed => ed.OriginalFileName)
                .IsRequired()
                .HasColumnType("varchar(500)");

            //Stored File name
            employeeDocumentBuilder.Property<string>(ed => ed.StoredFileName)
                .IsRequired()
                .HasColumnType("varchar(500)");

            //blob url
            employeeDocumentBuilder.Property<string>(ed => ed.BlobUrl)
                .IsRequired()
                .HasColumnType("varchar(max)");

            //document uploaded time
            employeeDocumentBuilder.Property<DateTime>(ed => ed.UploadedDate)
                .HasColumnType("DATETIME2");

            //fk
            employeeDocumentBuilder.HasOne(ed => ed.Employee)
               .WithMany(e => e.EmployeeDocuments)
               .HasForeignKey(ed => ed.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

            //fk doc category
            employeeDocumentBuilder.HasOne(ed => ed.DocumentType)
                .WithMany(dc => dc.EmployeeDocuments)
                .HasForeignKey(ed => ed.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            employeeDocumentBuilder
                .HasIndex(ed => ed.StoredFileName)
                .IsUnique();

            employeeDocumentBuilder
                .HasIndex(ed => new
                {
                    ed.EmployeeId,
                    ed.DocumentTypeId
                })
                .IsUnique();

            //[Document Category]
            //pk
            documentCategoryBuilder.ToTable("DocumentCategory").HasKey(dc => dc.Id);

            //id
            documentCategoryBuilder.Property<int>(dc => dc.Id)
               .ValueGeneratedOnAdd()
               .UseIdentityColumn(1, 1);

            //doc name
            documentCategoryBuilder.Property<string>(dc => dc.Name)
                .IsRequired()
               .HasColumnType("varchar(100)");

            //uniq doc category name
            documentCategoryBuilder
                .HasIndex(dc => dc.Name)
                .IsUnique();

            //[Document Type]
            documentTypeBuilder
                .ToTable("DocumentType")
                .HasKey(dt => dt.Id);

            //id
            documentTypeBuilder
                .Property(dt => dt.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1, 1);

            // Document Type Name
            documentTypeBuilder
                .Property(dt => dt.Name)
                .IsRequired()
                .HasColumnType("varchar(100)");

            //IsMandatory
            documentTypeBuilder
                .Property(dt => dt.IsMandatory)
                .IsRequired();

            // FK -> DocumentCategory
            documentTypeBuilder
                .HasOne(dt => dt.DocumentCategory)
                .WithMany(dc => dc.DocumentTypes)
                .HasForeignKey(dt => dt.DocumentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate document names inside same category
            documentTypeBuilder
                .HasIndex(dt => new
                {
                    dt.Name,
                    dt.DocumentCategoryId
                })
                .IsUnique();

        }
    }
}
