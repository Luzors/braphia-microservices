using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.MedicalManagement.Migrations
{
    /// <inheritdoc />
    public partial class TEstRelationWithMedical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_MedicalAnalysis_MedicalAnalysisId",
                table: "Test");

            migrationBuilder.AlterColumn<int>(
                name: "MedicalAnalysisId",
                table: "Test",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Test_MedicalAnalysis_MedicalAnalysisId",
                table: "Test",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_MedicalAnalysis_MedicalAnalysisId",
                table: "Test");

            migrationBuilder.AlterColumn<int>(
                name: "MedicalAnalysisId",
                table: "Test",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Test_MedicalAnalysis_MedicalAnalysisId",
                table: "Test",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id");
        }
    }
}
