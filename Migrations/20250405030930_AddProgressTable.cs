using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_WebH3.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastUpdate",
                table: "Progresses",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "CompletionPercentage",
                table: "Progresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Progresses",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAt",
                table: "Lessons",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionPercentage",
                table: "Progresses");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Progresses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Lessons");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdate",
                table: "Progresses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Lessons",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
