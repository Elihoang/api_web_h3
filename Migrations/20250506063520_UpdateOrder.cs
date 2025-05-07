using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_WebH3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Courses_CourseId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CourseId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CourseId",
                table: "Orders",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Courses_CourseId",
                table: "Orders",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
