using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActivelyInfrastructure.Migrations
{
    public partial class deletedSportTypeTableFromDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_SportType_SportId",
                table: "Game");

            migrationBuilder.DropTable(
                name: "SportType");

            migrationBuilder.DropIndex(
                name: "IX_Game_SportId",
                table: "Game");

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

            migrationBuilder.RenameColumn(
                name: "SportId",
                table: "Game",
                newName: "Sport");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "09bdd2f0-d7d2-4f73-87d1-0827c3771c10", "6", "VerifiedUser", "VerifiedUser" },
                    { "25778440-65df-4c96-af20-e1b7a57f1fb6", "5", "GameOwner", "GameOwner" },
                    { "49548689-7e86-4cff-ad34-0c9e75e8702d", "7", "User", "User" },
                    { "4a0b5d54-6413-404b-aa61-23949695c910", "2", "Developer", "Developer" },
                    { "5fc55304-63f7-4837-8bcb-e53d8b3001a0", "3", "Moderator", "Moderator" },
                    { "730f65b5-aa33-4fd5-9d6d-b01768b34944", "1", "Admin", "Admin" },
                    { "9ecc3a2e-5a91-4ad7-9203-f40786c91e00", "4", "PremiumUser", "PremiumUser" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09bdd2f0-d7d2-4f73-87d1-0827c3771c10");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25778440-65df-4c96-af20-e1b7a57f1fb6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49548689-7e86-4cff-ad34-0c9e75e8702d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a0b5d54-6413-404b-aa61-23949695c910");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5fc55304-63f7-4837-8bcb-e53d8b3001a0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "730f65b5-aa33-4fd5-9d6d-b01768b34944");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9ecc3a2e-5a91-4ad7-9203-f40786c91e00");

            migrationBuilder.RenameColumn(
                name: "Sport",
                table: "Game",
                newName: "SportId");

            migrationBuilder.CreateTable(
                name: "SportType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportType", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Game_SportId",
                table: "Game",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_SportType_SportId",
                table: "Game",
                column: "SportId",
                principalTable: "SportType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
