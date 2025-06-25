using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.Accounting.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvoiceNumberAndInsurerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Invoice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumber",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
