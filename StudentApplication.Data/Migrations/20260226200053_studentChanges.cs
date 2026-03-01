using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class studentChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearOfStudy",
                table: "Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfStudy",
                table: "Students");
        }
    }
}
