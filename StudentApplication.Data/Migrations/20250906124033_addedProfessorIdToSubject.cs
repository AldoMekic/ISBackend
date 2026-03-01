using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedProfessorIdToSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Professors_ProfessorId",
                table: "Subjects");

            migrationBuilder.AlterColumn<int>(
                name: "ProfessorId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Professors_ProfessorId",
                table: "Subjects",
                column: "ProfessorId",
                principalTable: "Professors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Professors_ProfessorId",
                table: "Subjects");

            migrationBuilder.AlterColumn<int>(
                name: "ProfessorId",
                table: "Subjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Professors_ProfessorId",
                table: "Subjects",
                column: "ProfessorId",
                principalTable: "Professors",
                principalColumn: "Id");
        }
    }
}
