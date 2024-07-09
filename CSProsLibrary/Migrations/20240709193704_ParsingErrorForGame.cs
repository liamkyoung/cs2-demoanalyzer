using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSProsLibrary.Migrations
{
    /// <inheritdoc />
    public partial class ParsingErrorForGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MatchHadParsingError",
                table: "Games",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchHadParsingError",
                table: "Games");
        }
    }
}
