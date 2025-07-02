using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Migrations
{
    /// <inheritdoc />
    public partial class Addischeckedtopatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIdChecked",
                table: "Appointments");

            migrationBuilder.AddColumn<bool>(
                name: "IsIdChecked",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIdChecked",
                table: "Patients");

            migrationBuilder.AddColumn<bool>(
                name: "IsIdChecked",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
