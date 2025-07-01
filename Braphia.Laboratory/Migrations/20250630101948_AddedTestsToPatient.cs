using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Laboratory.Migrations
{
    /// <inheritdoc />
    public partial class AddedTestsToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Test_PatientId",
                table: "Test",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Test_Patient_PatientId",
                table: "Test",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_Patient_PatientId",
                table: "Test");

            migrationBuilder.DropIndex(
                name: "IX_Test_PatientId",
                table: "Test");
        }
    }
}
