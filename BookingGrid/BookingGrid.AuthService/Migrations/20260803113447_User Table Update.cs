using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingGrid.AuthService.Migrations
{
    public partial class UserTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetOtp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetOtpExpiry",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetOtp",
                table: "Users",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetOtpExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
