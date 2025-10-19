using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class deleteitemswithpresciption : Migration
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrescriptionItem",
                table: "PrescriptionItem");

            migrationBuilder.RenameTable(
                name: "PrescriptionItem",
                newName: "PrescriptionItems");

            migrationBuilder.RenameIndex(
                name: "IX_PrescriptionItem_PrescriptionId",
                table: "PrescriptionItems",
                newName: "IX_PrescriptionItems_PrescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_PrescriptionItem_MedicationId",
                table: "PrescriptionItems",
                newName: "IX_PrescriptionItems_MedicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrescriptionItems",
                table: "PrescriptionItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Medications_MedicationId",
                table: "PrescriptionItems",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Prescriptions_PrescriptionId",
                table: "PrescriptionItems",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Medications_MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Prescriptions_PrescriptionId",
                table: "PrescriptionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrescriptionItems",
                table: "PrescriptionItems");

            migrationBuilder.RenameTable(
                name: "PrescriptionItems",
                newName: "PrescriptionItem");

            migrationBuilder.RenameIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItem",
                newName: "IX_PrescriptionItem_PrescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_PrescriptionItems_MedicationId",
                table: "PrescriptionItem",
                newName: "IX_PrescriptionItem_MedicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrescriptionItem",
                table: "PrescriptionItem",
                column: "Id");

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
    }
}
