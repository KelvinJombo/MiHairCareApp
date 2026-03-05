using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiHairCareApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StylistRatingsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserHairStyle");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HairStyleId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_AppUserId",
                table: "Ratings",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_HairStyleId",
                table: "AspNetUsers",
                column: "HairStyleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HairStyles_HairStyleId",
                table: "AspNetUsers",
                column: "HairStyleId",
                principalTable: "HairStyles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_AppUserId",
                table: "Ratings",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HairStyles_HairStyleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_AppUserId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_AppUserId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_HairStyleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "HairStyleId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "AppUserHairStyle",
                columns: table => new
                {
                    AvailableStylesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StylistsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserHairStyle", x => new { x.AvailableStylesId, x.StylistsId });
                    table.ForeignKey(
                        name: "FK_AppUserHairStyle_AspNetUsers_StylistsId",
                        column: x => x.StylistsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserHairStyle_HairStyles_AvailableStylesId",
                        column: x => x.AvailableStylesId,
                        principalTable: "HairStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserHairStyle_StylistsId",
                table: "AppUserHairStyle",
                column: "StylistsId");
        }
    }
}
