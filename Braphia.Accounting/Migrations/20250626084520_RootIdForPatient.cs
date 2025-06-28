using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Accounting.Migrations
{
    /// <inheritdoc />
    public partial class RootIdForPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice");

            migrationBuilder.AddColumn<int>(
                name: "InsurerId",
                table: "Patient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RootId",
                table: "Patient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "InsurerId",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_InsurerId",
                table: "Patient",
                column: "InsurerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice",
                column: "InsurerId",
                principalTable: "Insurer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Insurer_InsurerId",
                table: "Patient",
                column: "InsurerId",
                principalTable: "Insurer",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Insurer_InsurerId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_InsurerId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "InsurerId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Invoice");

            migrationBuilder.AlterColumn<int>(
                name: "InsurerId",
                table: "Invoice",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice",
                column: "InsurerId",
                principalTable: "Insurer",
                principalColumn: "Id");
        }
    }
}
