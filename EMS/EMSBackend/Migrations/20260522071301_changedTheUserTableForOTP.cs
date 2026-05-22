using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class changedTheUserTableForOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResetTokenExpiry",
                table: "User",
                newName: "PasswordResetOtpExpiry");

            migrationBuilder.RenameColumn(
                name: "PasswordResetToken",
                table: "User",
                newName: "PasswordResetOtp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordResetOtpExpiry",
                table: "User",
                newName: "ResetTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "PasswordResetOtp",
                table: "User",
                newName: "PasswordResetToken");
        }
    }
}
