using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdfromUserActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserActivities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserActivities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
