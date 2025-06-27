using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.MedicalManagement.Migrations
{
    /// <inheritdoc />
    public partial class fixxedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalAnalysisId1",
                table: "Prescription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "Prescription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhysicianId1",
                table: "Prescription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId1",
                table: "MedicalAnalysis",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "MedicalAnalysis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhysicianId1",
                table: "MedicalAnalysis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_MedicalAnalysisId1",
                table: "Prescription",
                column: "MedicalAnalysisId1");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PatientId1",
                table: "Prescription",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PhysicianId1",
                table: "Prescription",
                column: "PhysicianId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_AppointmentId1",
                table: "MedicalAnalysis",
                column: "AppointmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_PatientId1",
                table: "MedicalAnalysis",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_PhysicianId1",
                table: "MedicalAnalysis",
                column: "PhysicianId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId1",
                table: "MedicalAnalysis",
                column: "AppointmentId1",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Patient_PatientId1",
                table: "MedicalAnalysis",
                column: "PatientId1",
                principalTable: "Patient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Physician_PhysicianId1",
                table: "MedicalAnalysis",
                column: "PhysicianId1",
                principalTable: "Physician",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId1",
                table: "Prescription",
                column: "MedicalAnalysisId1",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Patient_PatientId1",
                table: "Prescription",
                column: "PatientId1",
                principalTable: "Patient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Physician_PhysicianId1",
                table: "Prescription",
                column: "PhysicianId1",
                principalTable: "Physician",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Patient_PatientId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Physician_PhysicianId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId1",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Patient_PatientId1",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Physician_PhysicianId1",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_MedicalAnalysisId1",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_PatientId1",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_PhysicianId1",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_MedicalAnalysis_AppointmentId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropIndex(
                name: "IX_MedicalAnalysis_PatientId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropIndex(
                name: "IX_MedicalAnalysis_PhysicianId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropColumn(
                name: "MedicalAnalysisId1",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "PhysicianId1",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "AppointmentId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "MedicalAnalysis");

            migrationBuilder.DropColumn(
                name: "PhysicianId1",
                table: "MedicalAnalysis");
        }
    }
}
