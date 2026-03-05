using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiHairCareApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModelsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StylePicturesLink",
                table: "StylistPortfolios");

            migrationBuilder.DropColumn(
                name: "StyleVideoLink",
                table: "StylistPortfolios");

            migrationBuilder.AddColumn<string>(
                name: "StylistPortfolioId",
                table: "HairStyles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StylistPortfolioId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeSlot",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndHour",
                table: "AspNetUsers",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartHour",
                table: "AspNetUsers",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_HairStyles_StylistPortfolioId",
                table: "HairStyles",
                column: "StylistPortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StylistPortfolioId",
                table: "Bookings",
                column: "StylistPortfolioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StylistPortfolios_StylistPortfolioId",
                table: "Bookings",
                column: "StylistPortfolioId",
                principalTable: "StylistPortfolios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HairStyles_StylistPortfolios_StylistPortfolioId",
                table: "HairStyles",
                column: "StylistPortfolioId",
                principalTable: "StylistPortfolios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StylistPortfolios_StylistPortfolioId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_HairStyles_StylistPortfolios_StylistPortfolioId",
                table: "HairStyles");

            migrationBuilder.DropIndex(
                name: "IX_HairStyles_StylistPortfolioId",
                table: "HairStyles");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StylistPortfolioId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StylistPortfolioId",
                table: "HairStyles");

            migrationBuilder.DropColumn(
                name: "StylistPortfolioId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TimeSlot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "StylePicturesLink",
                table: "StylistPortfolios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StyleVideoLink",
                table: "StylistPortfolios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
