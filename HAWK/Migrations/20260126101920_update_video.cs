using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAWK.Migrations
{
    /// <inheritdoc />
    public partial class update_video : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "video",
                table: "Sliders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "video",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "video",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "video",
                table: "Projects");
        }
    }
}
