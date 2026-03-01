using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedBoolValuesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfessor",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStudent",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfessor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsStudent",
                table: "Users");
        }
    }
}
