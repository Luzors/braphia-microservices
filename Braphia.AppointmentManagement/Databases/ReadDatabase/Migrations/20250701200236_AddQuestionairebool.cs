using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionairebool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPreAppointmentQuestionnaireFilled",
                table: "AppointmentViewQueryModels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreAppointmentQuestionnaireFilled",
                table: "AppointmentViewQueryModels");
        }
    }
}
