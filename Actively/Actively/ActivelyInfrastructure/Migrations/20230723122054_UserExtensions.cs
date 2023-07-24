using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActivelyInfrastructure.Migrations
{
    public partial class UserExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "15b74f69-8d6d-4b4d-9f62-62768b614568");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39ab5e7a-10f6-45f3-80a2-7e23d181e236");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71c86bbd-7292-4ea0-91ae-9b7a178862ee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87b28e5a-d472-465a-8482-a0490354718c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "baf1a1b6-2ac5-4da4-9aba-d65b74f37fbb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c112a933-91ad-431c-b072-18bb77620038");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9cc8963-7c3e-4f66-9eb2-3bd6e450cc80");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAvatar",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2eefbb56-73ca-4ef6-a9c7-4a897e784cac", "6", "VerifiedUser", "VerifiedUser" },
                    { "3cdf2f72-efeb-4250-8ed8-4c5a3029f210", "5", "GameOwner", "GameOwner" },
                    { "8c11250c-fdfd-4ec3-8dc1-0a86912a859c", "7", "User", "User" },
                    { "bd188919-d376-4698-aee0-ea5643d07ed6", "4", "PremiumUser", "PremiumUser" },
                    { "c4e3a9e6-544a-4db7-80a9-d8169d3561df", "1", "Admin", "Admin" },
                    { "ca888d36-f4fb-4402-8650-6c9a09079a79", "2", "Developer", "Developer" },
                    { "ef16d7d6-a024-4486-a33b-a093a4dbf636", "3", "Moderator", "Moderator" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2eefbb56-73ca-4ef6-a9c7-4a897e784cac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3cdf2f72-efeb-4250-8ed8-4c5a3029f210");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c11250c-fdfd-4ec3-8dc1-0a86912a859c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd188919-d376-4698-aee0-ea5643d07ed6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4e3a9e6-544a-4db7-80a9-d8169d3561df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca888d36-f4fb-4402-8650-6c9a09079a79");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef16d7d6-a024-4486-a33b-a093a4dbf636");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserAvatar",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "15b74f69-8d6d-4b4d-9f62-62768b614568", "3", "Moderator", "Moderator" },
                    { "39ab5e7a-10f6-45f3-80a2-7e23d181e236", "7", "User", "User" },
                    { "71c86bbd-7292-4ea0-91ae-9b7a178862ee", "4", "PremiumUser", "PremiumUser" },
                    { "87b28e5a-d472-465a-8482-a0490354718c", "1", "Admin", "Admin" },
                    { "baf1a1b6-2ac5-4da4-9aba-d65b74f37fbb", "5", "GameOwner", "GameOwner" },
                    { "c112a933-91ad-431c-b072-18bb77620038", "6", "VerifiedUser", "VerifiedUser" },
                    { "c9cc8963-7c3e-4f66-9eb2-3bd6e450cc80", "2", "Developer", "Developer" }
                });
        }
    }
}
