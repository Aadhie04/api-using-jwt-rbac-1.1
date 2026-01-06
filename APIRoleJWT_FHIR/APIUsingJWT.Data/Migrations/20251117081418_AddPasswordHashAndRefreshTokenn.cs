using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIUsingJWT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashAndRefreshTokenn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "age",
                table: "Staffs",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Staffs",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_Password",
                table: "Staffs",
                newName: "IX_Staffs_PasswordHash");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Salary",
                table: "Staffs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Staffs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Advertisements",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Staffs");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "Staffs",
                newName: "age");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Staffs",
                newName: "Password");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_PasswordHash",
                table: "Staffs",
                newName: "IX_Staffs_Password");

            migrationBuilder.AlterColumn<int>(
                name: "Salary",
                table: "Staffs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Advertisements",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }
    }
}
