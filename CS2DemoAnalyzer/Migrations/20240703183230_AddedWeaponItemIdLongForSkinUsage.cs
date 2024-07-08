using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS2DemoAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeaponItemIdLongForSkinUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeaponItems_Skins_SkinId",
                table: "WeaponItems");

            migrationBuilder.AlterColumn<int>(
                name: "SkinId",
                table: "WeaponItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponItems_Skins_SkinId",
                table: "WeaponItems",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeaponItems_Skins_SkinId",
                table: "WeaponItems");

            migrationBuilder.AlterColumn<int>(
                name: "SkinId",
                table: "WeaponItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponItems_Skins_SkinId",
                table: "WeaponItems",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
