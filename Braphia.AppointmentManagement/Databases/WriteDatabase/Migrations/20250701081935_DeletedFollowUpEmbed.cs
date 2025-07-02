using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Migrations
{
    /// <inheritdoc />
    public partial class DeletedFollowUpEmbed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Appointments_FollowUpAppointmentId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_FollowUpAppointmentId",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FollowUpAppointmentId",
                table: "Appointments",
                column: "FollowUpAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Appointments_FollowUpAppointmentId",
                table: "Appointments",
                column: "FollowUpAppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }
    }
}
