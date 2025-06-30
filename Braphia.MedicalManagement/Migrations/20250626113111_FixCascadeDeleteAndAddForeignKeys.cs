using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.MedicalManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteAndAddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId",
                table: "MedicalAnalysis");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Patient_WrittenForId",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Physician_WrittenById",
                table: "Prescription");

            migrationBuilder.RenameColumn(
                name: "WrittenForId",
                table: "Prescription",
                newName: "PhysicianId");

            migrationBuilder.RenameColumn(
                name: "WrittenById",
                table: "Prescription",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_WrittenForId",
                table: "Prescription",
                newName: "IX_Prescription_PhysicianId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_WrittenById",
                table: "Prescription",
                newName: "IX_Prescription_PatientId");

            migrationBuilder.AlterColumn<int>(
                name: "MedicalAnalysisId",
                table: "Prescription",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "MedicalAnalysis",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RootId",
                table: "Appointment",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId",
                table: "MedicalAnalysis",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId",
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

            migrationBuilder.RenameColumn(
                name: "PhysicianId",
                table: "Prescription",
                newName: "WrittenForId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Prescription",
                newName: "WrittenById");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_PhysicianId",
                table: "Prescription",
                newName: "IX_Prescription_WrittenForId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_PatientId",
                table: "Prescription",
                newName: "IX_Prescription_WrittenById");

            migrationBuilder.AlterColumn<int>(
                name: "MedicalAnalysisId",
                table: "Prescription",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "MedicalAnalysis",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RootId",
                table: "Appointment",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalAnalysis_Appointment_AppointmentId",
                table: "MedicalAnalysis",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Patient_WrittenForId",
                table: "Prescription",
                column: "WrittenForId",
                principalTable: "Patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Physician_WrittenById",
                table: "Prescription",
                column: "WrittenById",
                principalTable: "Physician",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
