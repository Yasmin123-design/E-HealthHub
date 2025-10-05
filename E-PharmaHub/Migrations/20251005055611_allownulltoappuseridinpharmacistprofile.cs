using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class allownulltoappuseridinpharmacistprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacists_AspNetUsers_AppUserId",
                table: "Pharmacists");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacists_AppUserId",
                table: "Pharmacists");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Pharmacists",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Pharmacists",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacists_AppUserId",
                table: "Pharmacists",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacists_AspNetUsers_AppUserId",
                table: "Pharmacists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacists_AspNetUsers_AppUserId",
                table: "Pharmacists");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacists_AppUserId",
                table: "Pharmacists");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Pharmacists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Pharmacists",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacists_AppUserId",
                table: "Pharmacists",
                column: "AppUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacists_AspNetUsers_AppUserId",
                table: "Pharmacists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
