using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApprovedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Admins_ApprovedByAdminId",
                table: "Professors");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminId",
                table: "Professors",
                newName: "ApprovedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Professors_ApprovedByAdminId",
                table: "Professors",
                newName: "IX_Professors_ApprovedByUserId");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Professors",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_AdminId",
                table: "Professors",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Admins_AdminId",
                table: "Professors",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Users_ApprovedByUserId",
                table: "Professors",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Admins_AdminId",
                table: "Professors");

            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Users_ApprovedByUserId",
                table: "Professors");

            migrationBuilder.DropIndex(
                name: "IX_Professors_AdminId",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Professors");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserId",
                table: "Professors",
                newName: "ApprovedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Professors_ApprovedByUserId",
                table: "Professors",
                newName: "IX_Professors_ApprovedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Admins_ApprovedByAdminId",
                table: "Professors",
                column: "ApprovedByAdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
