using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class addphamasist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacistProfile_AspNetUsers_AppUserId",
                table: "PharmacistProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_PharmacistProfile_Pharmacies_PharmacyId",
                table: "PharmacistProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacistProfile",
                table: "PharmacistProfile");

            migrationBuilder.RenameTable(
                name: "PharmacistProfile",
                newName: "Pharmacists");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacistProfile_PharmacyId",
                table: "Pharmacists",
                newName: "IX_Pharmacists_PharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacistProfile_AppUserId",
                table: "Pharmacists",
                newName: "IX_Pharmacists_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pharmacists",
                table: "Pharmacists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacists_AspNetUsers_AppUserId",
                table: "Pharmacists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacists_Pharmacies_PharmacyId",
                table: "Pharmacists",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacists_AspNetUsers_AppUserId",
                table: "Pharmacists");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacists_Pharmacies_PharmacyId",
                table: "Pharmacists");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pharmacists",
                table: "Pharmacists");

            migrationBuilder.RenameTable(
                name: "Pharmacists",
                newName: "PharmacistProfile");

            migrationBuilder.RenameIndex(
                name: "IX_Pharmacists_PharmacyId",
                table: "PharmacistProfile",
                newName: "IX_PharmacistProfile_PharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_Pharmacists_AppUserId",
                table: "PharmacistProfile",
                newName: "IX_PharmacistProfile_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacistProfile",
                table: "PharmacistProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacistProfile_AspNetUsers_AppUserId",
                table: "PharmacistProfile",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacistProfile_Pharmacies_PharmacyId",
                table: "PharmacistProfile",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
