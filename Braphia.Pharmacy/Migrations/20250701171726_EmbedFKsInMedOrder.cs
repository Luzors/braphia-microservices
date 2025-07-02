using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Pharmacy.Migrations
{
    /// <inheritdoc />
    public partial class EmbedFKsInMedOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrder_PharmacyId",
                table: "MedicationOrder",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationOrder_Pharmacy_PharmacyId",
                table: "MedicationOrder",
                column: "PharmacyId",
                principalTable: "Pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationOrder_Pharmacy_PharmacyId",
                table: "MedicationOrder");

            migrationBuilder.DropIndex(
                name: "IX_MedicationOrder_PharmacyId",
                table: "MedicationOrder");
        }
    }
}
