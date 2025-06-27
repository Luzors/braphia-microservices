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

            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "TestName",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "TestStatus",
                table: "Tests",
                newName: "PatientId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tests",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "Tests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Tests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CentralLaboratories",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Appointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Tests",
                newName: "TestStatus");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Tests",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "Tests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CentralLaboratoryId",
                table: "Tests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "Tests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TestName",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CentralLaboratories",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_AppointmentId",
                table: "Tests",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CentralLaboratoryId",
                table: "Tests",
                column: "CentralLaboratoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Appointments_AppointmentId",
                table: "Tests",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_CentralLaboratories_CentralLaboratoryId",
                table: "Tests",
                column: "CentralLaboratoryId",
                principalTable: "CentralLaboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
