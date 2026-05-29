using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class addedFrildinDocType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                table: "DocumentType",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMandatory",
                table: "DocumentType");
        }
    }
}
