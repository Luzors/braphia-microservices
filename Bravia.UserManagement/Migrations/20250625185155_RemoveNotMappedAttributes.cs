using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotMappedAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Referral_GeneralPracticionerId",
                table: "Referral",
                column: "GeneralPracticionerId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral_PatientId",
                table: "Referral",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referral_GeneralPracticioner_GeneralPracticionerId",
                table: "Referral",
                column: "GeneralPracticionerId",
                principalTable: "GeneralPracticioner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Referral_Patient_PatientId",
                table: "Referral",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referral_GeneralPracticioner_GeneralPracticionerId",
                table: "Referral");

            migrationBuilder.DropForeignKey(
                name: "FK_Referral_Patient_PatientId",
                table: "Referral");

            migrationBuilder.DropIndex(
                name: "IX_Referral_GeneralPracticionerId",
                table: "Referral");

            migrationBuilder.DropIndex(
                name: "IX_Referral_PatientId",
                table: "Referral");
        }
    }
}
