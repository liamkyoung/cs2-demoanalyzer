using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS2DemoAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class AddSkinPropertyToWeaponId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WeaponItems_SkinId",
                table: "WeaponItems",
                column: "SkinId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponItems_Skins_SkinId",
                table: "WeaponItems",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeaponItems_Skins_SkinId",
                table: "WeaponItems");

            migrationBuilder.DropIndex(
                name: "IX_WeaponItems_SkinId",
                table: "WeaponItems");
        }
    }
}
