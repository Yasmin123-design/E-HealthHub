using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class adddescriptionwarningcompositionpropertiesinmediciationmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Composition",
                table: "Medications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Medications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DirectionsForUse",
                table: "Medications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotSuitableFor",
                table: "Medications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuitableFor",
                table: "Medications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Warning",
                table: "Medications",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Composition",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DirectionsForUse",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "NotSuitableFor",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "SuitableFor",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Warning",
                table: "Medications");
        }
    }
}
