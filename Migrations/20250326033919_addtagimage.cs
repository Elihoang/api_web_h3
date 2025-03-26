using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_WebH3.Migrations
{
    /// <inheritdoc />
    public partial class addtagimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "Posts");
        }
    }
}
