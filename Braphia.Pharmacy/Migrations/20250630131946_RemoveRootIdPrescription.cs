using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Pharmacy.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRootIdPrescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Prescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RootId",
                table: "Prescription",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
