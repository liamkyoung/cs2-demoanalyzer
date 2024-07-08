using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS2DemoAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeaponAndNameUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skins_WeaponId",
                table: "Skins");

            migrationBuilder.AlterColumn<long>(
                name: "WeaponItemId",
                table: "SkinUsages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_WeaponId_Name",
                table: "Skins",
                columns: new[] { "WeaponId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skins_WeaponId_Name",
                table: "Skins");

            migrationBuilder.AlterColumn<int>(
                name: "WeaponItemId",
                table: "SkinUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Skins_WeaponId",
                table: "Skins",
                column: "WeaponId");
        }
    }
}
