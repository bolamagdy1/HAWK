using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAWK.Migrations
{
    /// <inheritdoc />
    public partial class AddSliderLocationID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectImage_Projects_ProjectId",
                table: "ProjectImage");

            migrationBuilder.DropForeignKey(
                name: "FK_SliderImage_Sliders_SliderId",
                table: "SliderImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SliderImage",
                table: "SliderImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectImage",
                table: "ProjectImage");

            migrationBuilder.RenameTable(
                name: "SliderImage",
                newName: "SliderImages");

            migrationBuilder.RenameTable(
                name: "ProjectImage",
                newName: "ProjectImages");

            migrationBuilder.RenameIndex(
                name: "IX_SliderImage_SliderId",
                table: "SliderImages",
                newName: "IX_SliderImages_SliderId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectImage_ProjectId",
                table: "ProjectImages",
                newName: "IX_ProjectImages_ProjectId");

            migrationBuilder.AddColumn<int>(
                name: "SliderLocationID",
                table: "Sliders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SliderImages",
                table: "SliderImages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectImages",
                table: "ProjectImages",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectImages_Projects_ProjectId",
                table: "ProjectImages",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SliderImages_Sliders_SliderId",
                table: "SliderImages",
                column: "SliderId",
                principalTable: "Sliders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectImages_Projects_ProjectId",
                table: "ProjectImages");

            migrationBuilder.DropForeignKey(
                name: "FK_SliderImages_Sliders_SliderId",
                table: "SliderImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SliderImages",
                table: "SliderImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectImages",
                table: "ProjectImages");

            migrationBuilder.DropColumn(
                name: "SliderLocationID",
                table: "Sliders");

            migrationBuilder.RenameTable(
                name: "SliderImages",
                newName: "SliderImage");

            migrationBuilder.RenameTable(
                name: "ProjectImages",
                newName: "ProjectImage");

            migrationBuilder.RenameIndex(
                name: "IX_SliderImages_SliderId",
                table: "SliderImage",
                newName: "IX_SliderImage_SliderId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectImages_ProjectId",
                table: "ProjectImage",
                newName: "IX_ProjectImage_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SliderImage",
                table: "SliderImage",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectImage",
                table: "ProjectImage",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectImage_Projects_ProjectId",
                table: "ProjectImage",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SliderImage_Sliders_SliderId",
                table: "SliderImage",
                column: "SliderId",
                principalTable: "Sliders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
