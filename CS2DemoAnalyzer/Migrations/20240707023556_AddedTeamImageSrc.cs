using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS2DemoAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class AddedTeamImageSrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageSrc",
                table: "Teams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageSrc",
                table: "Teams");
        }
    }
}
