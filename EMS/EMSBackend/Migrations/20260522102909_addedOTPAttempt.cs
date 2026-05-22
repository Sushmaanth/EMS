using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class addedOTPAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OtpFailedAttempts",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpFailedAttempts",
                table: "User");
        }
    }
}
