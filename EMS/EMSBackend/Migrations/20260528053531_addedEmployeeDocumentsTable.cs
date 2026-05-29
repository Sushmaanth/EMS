using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class addedEmployeeDocumentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(500)", nullable: false),
                    BlobUrl = table.Column<string>(type: "varchar(max)", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDocument_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocument_EmployeeId",
                table: "EmployeeDocument",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeDocument");
        }
    }
}
