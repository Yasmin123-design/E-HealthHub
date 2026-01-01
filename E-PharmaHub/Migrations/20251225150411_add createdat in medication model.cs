using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class addcreatedatinmedicationmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InventoryItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InventoryItems");
        }
    }
}
