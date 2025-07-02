using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Migrations
{
    /// <inheritdoc />
    public partial class QuestionaireBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPreAppointmentQuestionnaireFilled",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreAppointmentQuestionnaireFilled",
                table: "Appointments");
        }
    }
}
