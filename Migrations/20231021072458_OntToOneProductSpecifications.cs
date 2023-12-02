using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileWeb.Migrations
{
    /// <inheritdoc />
    public partial class OntToOneProductSpecifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Specifications_SpecificationsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecificationsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecificationsId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Specifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_ProductId",
                table: "Specifications",
                column: "ProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Specifications_Products_ProductId",
                table: "Specifications",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specifications_Products_ProductId",
                table: "Specifications");

            migrationBuilder.DropIndex(
                name: "IX_Specifications_ProductId",
                table: "Specifications");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Specifications");

            migrationBuilder.AddColumn<int>(
                name: "SpecificationsId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecificationsId",
                table: "Products",
                column: "SpecificationsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Specifications_SpecificationsId",
                table: "Products",
                column: "SpecificationsId",
                principalTable: "Specifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
