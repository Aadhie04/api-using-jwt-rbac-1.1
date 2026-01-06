using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIUsingJWT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenHashProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Users",
                newName: "RefreshTokenHash");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Staffs",
                newName: "RefreshTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "Users",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "Staffs",
                newName: "RefreshToken");
        }
    }
}
