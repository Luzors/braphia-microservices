using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionaireV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreAppointmentQuestionnaireJson",
                table: "AppointmentViewQueryModels",
                newName: "PreAppointmentQuestionnaire");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreAppointmentQuestionnaire",
                table: "AppointmentViewQueryModels",
                newName: "PreAppointmentQuestionnaireJson");
        }
    }
}
