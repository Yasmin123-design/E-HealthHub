using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class addDosageFormTypeandStrengthUnitandGenderSuitabilityinmedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "Medications");

            migrationBuilder.AddColumn<int>(
                name: "DosageFormType",
                table: "Medications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GenderSuitability",
                table: "Medications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StrengthUnit",
                table: "Medications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosageFormType",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "GenderSuitability",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "StrengthUnit",
                table: "Medications");

            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "Medications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
