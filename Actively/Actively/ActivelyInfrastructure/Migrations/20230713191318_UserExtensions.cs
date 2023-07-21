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
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
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
                    { "10c57d0e-666e-4a6f-a8d1-57f088cd81af", "4", "PremiumUser", "PremiumUser" },
                    { "32070027-4dac-4f5b-bc38-8e10f21ed09d", "3", "Moderator", "Moderator" },
                    { "72a99633-1562-49e0-bd65-eb8fbebbc5e6", "6", "VerifiedUser", "VerifiedUser" },
                    { "8dc6f9c8-a106-467e-a6e8-db431853dd49", "2", "Developer", "Developer" },
                    { "8f53b579-103f-481c-81d3-4bdce7cee5dc", "5", "GameOwner", "GameOwner" },
                    { "bfee928f-84e3-4b33-bc7d-ea89c23c6c16", "1", "Admin", "Admin" },
                    { "e15f2e55-9f23-4f7e-aac8-64122aa1cdcf", "7", "User", "User" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "10c57d0e-666e-4a6f-a8d1-57f088cd81af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "32070027-4dac-4f5b-bc38-8e10f21ed09d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "72a99633-1562-49e0-bd65-eb8fbebbc5e6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8dc6f9c8-a106-467e-a6e8-db431853dd49");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f53b579-103f-481c-81d3-4bdce7cee5dc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bfee928f-84e3-4b33-bc7d-ea89c23c6c16");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e15f2e55-9f23-4f7e-aac8-64122aa1cdcf");

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
