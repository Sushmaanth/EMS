using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class updatedEmployeeDocumentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "EmployeeDocument",
                newName: "OriginalFileName");

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "EmployeeDocument",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "EmployeeDocument");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "EmployeeDocument",
                newName: "FileName");
        }
    }
}
