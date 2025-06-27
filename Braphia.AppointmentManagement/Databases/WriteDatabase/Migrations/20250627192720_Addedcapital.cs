using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Migrations
{
    /// <inheritdoc />
    public partial class Addedcapital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_Referral_ReferralId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_appointments_FollowUpAppointmentId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_patients_PatientId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_physicians_PhysicianId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_receptionists_ReceptionistId",
                table: "appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_receptionists",
                table: "receptionists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_physicians",
                table: "physicians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_patients",
                table: "patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_appointments",
                table: "appointments");

            migrationBuilder.RenameTable(
                name: "receptionists",
                newName: "Receptionists");

            migrationBuilder.RenameTable(
                name: "physicians",
                newName: "Physicians");

            migrationBuilder.RenameTable(
                name: "patients",
                newName: "Patients");

            migrationBuilder.RenameTable(
                name: "appointments",
                newName: "Appointments");

            migrationBuilder.RenameIndex(
                name: "IX_appointments_ReferralId",
                table: "Appointments",
                newName: "IX_Appointments_ReferralId");

            migrationBuilder.RenameIndex(
                name: "IX_appointments_ReceptionistId",
                table: "Appointments",
                newName: "IX_Appointments_ReceptionistId");

            migrationBuilder.RenameIndex(
                name: "IX_appointments_PhysicianId",
                table: "Appointments",
                newName: "IX_Appointments_PhysicianId");

            migrationBuilder.RenameIndex(
                name: "IX_appointments_PatientId",
                table: "Appointments",
                newName: "IX_Appointments_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_appointments_FollowUpAppointmentId",
                table: "Appointments",
                newName: "IX_Appointments_FollowUpAppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receptionists",
                table: "Receptionists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Physicians",
                table: "Physicians",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Appointments_FollowUpAppointmentId",
                table: "Appointments",
                column: "FollowUpAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Physicians_PhysicianId",
                table: "Appointments",
                column: "PhysicianId",
                principalTable: "Physicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Receptionists_ReceptionistId",
                table: "Appointments",
                column: "ReceptionistId",
                principalTable: "Receptionists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Referral_ReferralId",
                table: "Appointments",
                column: "ReferralId",
                principalTable: "Referral",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Appointments_FollowUpAppointmentId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Physicians_PhysicianId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Receptionists_ReceptionistId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Referral_ReferralId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receptionists",
                table: "Receptionists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Physicians",
                table: "Physicians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "Receptionists",
                newName: "receptionists");

            migrationBuilder.RenameTable(
                name: "Physicians",
                newName: "physicians");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "patients");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "appointments");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ReferralId",
                table: "appointments",
                newName: "IX_appointments_ReferralId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ReceptionistId",
                table: "appointments",
                newName: "IX_appointments_ReceptionistId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_PhysicianId",
                table: "appointments",
                newName: "IX_appointments_PhysicianId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_PatientId",
                table: "appointments",
                newName: "IX_appointments_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_FollowUpAppointmentId",
                table: "appointments",
                newName: "IX_appointments_FollowUpAppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_receptionists",
                table: "receptionists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_physicians",
                table: "physicians",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_patients",
                table: "patients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_appointments",
                table: "appointments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_Referral_ReferralId",
                table: "appointments",
                column: "ReferralId",
                principalTable: "Referral",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_appointments_FollowUpAppointmentId",
                table: "appointments",
                column: "FollowUpAppointmentId",
                principalTable: "appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_patients_PatientId",
                table: "appointments",
                column: "PatientId",
                principalTable: "patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_physicians_PhysicianId",
                table: "appointments",
                column: "PhysicianId",
                principalTable: "physicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_receptionists_ReceptionistId",
                table: "appointments",
                column: "ReceptionistId",
                principalTable: "receptionists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
