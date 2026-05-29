using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class addedDocCategoryandDocTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocument_EmployeeId",
                table: "EmployeeDocument");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "EmployeeDocument");

            migrationBuilder.AlterColumn<string>(
                name: "StoredFileName",
                table: "EmployeeDocument",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "EmployeeDocument",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DocumentCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    DocumentCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentType_DocumentCategory_DocumentCategoryId",
                        column: x => x.DocumentCategoryId,
                        principalTable: "DocumentCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocument_DocumentTypeId",
                table: "EmployeeDocument",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocument_EmployeeId_DocumentTypeId",
                table: "EmployeeDocument",
                columns: new[] { "EmployeeId", "DocumentTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocument_StoredFileName",
                table: "EmployeeDocument",
                column: "StoredFileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategory_Name",
                table: "DocumentCategory",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentType_DocumentCategoryId",
                table: "DocumentType",
                column: "DocumentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentType_Name_DocumentCategoryId",
                table: "DocumentType",
                columns: new[] { "Name", "DocumentCategoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocument_DocumentType_DocumentTypeId",
                table: "EmployeeDocument",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDocument_DocumentType_DocumentTypeId",
                table: "EmployeeDocument");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "DocumentCategory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocument_DocumentTypeId",
                table: "EmployeeDocument");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocument_EmployeeId_DocumentTypeId",
                table: "EmployeeDocument");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDocument_StoredFileName",
                table: "EmployeeDocument");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "EmployeeDocument");

            migrationBuilder.AlterColumn<string>(
                name: "StoredFileName",
                table: "EmployeeDocument",
                type: "varchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)");

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "EmployeeDocument",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocument_EmployeeId",
                table: "EmployeeDocument",
                column: "EmployeeId");
        }
    }
}
