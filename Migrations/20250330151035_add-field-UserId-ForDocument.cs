using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlideCloud.Migrations
{
    /// <inheritdoc />
    public partial class addfieldUserIdForDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Pagination",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Pagination",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UserId",
                table: "Pagination",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UserId",
                table: "Pagination");

            migrationBuilder.DropIndex(
                name: "IX_Documents_UserId",
                table: "Pagination");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Pagination");
        }
    }
}
