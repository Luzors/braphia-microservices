using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Accounting.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceRemovedPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice",
                column: "InsurerId",
                principalTable: "Insurer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Insurer_InsurerId",
                table: "Invoice",
                column: "InsurerId",
                principalTable: "Insurer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
