using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Pharmacy.Migrations
{
    /// <inheritdoc />
    public partial class PrescriptionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dose",
                table: "Prescription",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Medicine",
                table: "Prescription",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "Prescription",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dose",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Medicine",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Prescription");
        }
    }
}
