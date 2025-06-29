using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Laboratory.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Appointments_AppointmentId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_CentralLaboratories_CentralLaboratoryId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_AppointmentId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_CentralLaboratoryId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "CentralLaboratoryId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Tests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CentralLaboratoryId",
                table: "Tests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tests",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "CentralLaboratoryId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Tests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CentralLaboratoryId",
                table: "Tests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tests",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }
    }
}
