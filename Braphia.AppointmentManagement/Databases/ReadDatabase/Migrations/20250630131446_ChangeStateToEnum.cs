using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStateToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateName",
                table: "AppointmentViewQueryModels");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "AppointmentViewQueryModels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "AppointmentViewQueryModels");

            migrationBuilder.AddColumn<string>(
                name: "StateName",
                table: "AppointmentViewQueryModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
