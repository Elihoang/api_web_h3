using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_WebH3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUrlImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "Courses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "Courses");
        }
    }
}
