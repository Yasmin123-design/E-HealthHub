using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class allowaddnewmedicineinpresiptionnotexistinmedicationmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Medications_MedicationId",
                table: "PrescriptionItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.AlterColumn<int>(
                name: "PrescriptionId",
                table: "PrescriptionItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MedicationId",
                table: "PrescriptionItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PrescriptionItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Medications_MedicationId",
                table: "PrescriptionItem",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Medications_MedicationId",
                table: "PrescriptionItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PrescriptionItem");

            migrationBuilder.AlterColumn<int>(
                name: "PrescriptionId",
                table: "PrescriptionItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MedicationId",
                table: "PrescriptionItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Medications_MedicationId",
                table: "PrescriptionItem",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Prescriptions_PrescriptionId",
                table: "PrescriptionItem",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
