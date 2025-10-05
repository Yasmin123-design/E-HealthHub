using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class connectclinicwithdoctorprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClinicName",
                table: "DoctorProfile");

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "DoctorProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfile_ClinicId",
                table: "DoctorProfile",
                column: "ClinicId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfile_Clinics_ClinicId",
                table: "DoctorProfile",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfile_Clinics_ClinicId",
                table: "DoctorProfile");

            migrationBuilder.DropIndex(
                name: "IX_DoctorProfile_ClinicId",
                table: "DoctorProfile");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "DoctorProfile");

            migrationBuilder.AddColumn<string>(
                name: "ClinicName",
                table: "DoctorProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
