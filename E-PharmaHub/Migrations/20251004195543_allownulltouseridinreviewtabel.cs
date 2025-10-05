using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class allownulltouseridinreviewtabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfile_AspNetUsers_AppUserId",
                table: "DoctorProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfile_Clinics_ClinicId",
                table: "DoctorProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorProfile",
                table: "DoctorProfile");

            migrationBuilder.RenameTable(
                name: "DoctorProfile",
                newName: "DoctorProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorProfile_ClinicId",
                table: "DoctorProfiles",
                newName: "IX_DoctorProfiles_ClinicId");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorProfile_AppUserId",
                table: "DoctorProfiles",
                newName: "IX_DoctorProfiles_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorProfiles",
                table: "DoctorProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_AspNetUsers_AppUserId",
                table: "DoctorProfiles",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Clinics_ClinicId",
                table: "DoctorProfiles",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_AspNetUsers_AppUserId",
                table: "DoctorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Clinics_ClinicId",
                table: "DoctorProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorProfiles",
                table: "DoctorProfiles");

            migrationBuilder.RenameTable(
                name: "DoctorProfiles",
                newName: "DoctorProfile");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorProfiles_ClinicId",
                table: "DoctorProfile",
                newName: "IX_DoctorProfile_ClinicId");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorProfiles_AppUserId",
                table: "DoctorProfile",
                newName: "IX_DoctorProfile_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorProfile",
                table: "DoctorProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfile_AspNetUsers_AppUserId",
                table: "DoctorProfile",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfile_Clinics_ClinicId",
                table: "DoctorProfile",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
