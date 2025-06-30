using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.MedicalManagement.Migrations
{
    /// <inheritdoc />
    public partial class PrescRelationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id");
        }
    }
}
