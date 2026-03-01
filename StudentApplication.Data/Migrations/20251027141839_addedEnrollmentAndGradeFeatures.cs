using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedEnrollmentAndGradeFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AnnulmentRequested",
                table: "Grades",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AnnulmentRequestedAt",
                table: "Grades",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnulmentRequested",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "AnnulmentRequestedAt",
                table: "Grades");
        }
    }
}
