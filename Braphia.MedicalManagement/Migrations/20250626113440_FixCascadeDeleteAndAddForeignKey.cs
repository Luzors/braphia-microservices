using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.MedicalManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteAndAddForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId",
                table: "MedicalAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Physician_PhysicianId",
                table: "MedicalAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Patient_PatientId",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Physician_PhysicianId",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_PatientId",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_PhysicianId",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_MedicalAnalysis_AppointmentId",
                table: "MedicalAnalysis");

            migrationBuilder.DropIndex(
                name: "IX_MedicalAnalysis_PhysicianId",
                table: "MedicalAnalysis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Prescription_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PatientId",
                table: "Prescription",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PhysicianId",
                table: "Prescription",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_AppointmentId",
                table: "MedicalAnalysis",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_PhysicianId",
                table: "MedicalAnalysis",
                column: "PhysicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId",
                table: "MedicalAnalysis",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Physician_PhysicianId",
                table: "MedicalAnalysis",
                column: "PhysicianId",
                principalTable: "Physician",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Patient_PatientId",
                table: "Prescription",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Physician_PhysicianId",
                table: "Prescription",
                column: "PhysicianId",
                principalTable: "Physician",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
