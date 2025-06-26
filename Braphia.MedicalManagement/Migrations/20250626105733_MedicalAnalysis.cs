using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.MedicalManagement.Migrations
{
    /// <inheritdoc />
    public partial class MedicalAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RootId",
                table: "Receptionist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicalAnalysisId",
                table: "Prescription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RootId",
                table: "Physician",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RootId",
                table: "Patient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RootId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalAnalysis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PhysicianId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalAnalysis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalAnalysis_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MedicalAnalysis_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalAnalysis_Physician_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Physician",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_AppointmentId",
                table: "MedicalAnalysis",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_PatientId",
                table: "MedicalAnalysis",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAnalysis_PhysicianId",
                table: "MedicalAnalysis",
                column: "PhysicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription",
                column: "MedicalAnalysisId",
                principalTable: "MedicalAnalysis",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_MedicalAnalysis_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.DropTable(
                name: "MedicalAnalysis");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Receptionist");

            migrationBuilder.DropColumn(
                name: "MedicalAnalysisId",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Physician");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Patient");
        }
    }
}
