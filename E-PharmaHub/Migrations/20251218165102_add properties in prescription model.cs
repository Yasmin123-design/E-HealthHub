using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class addpropertiesinprescriptionmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Medications_MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "PrescriptionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicationStrength",
                table: "PrescriptionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PrescriptionItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "MedicationStrength",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PrescriptionItems");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "PrescriptionItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_MedicationId",
                table: "PrescriptionItems",
                column: "MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Medications_MedicationId",
                table: "PrescriptionItems",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id");
        }
    }
}
