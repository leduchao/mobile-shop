using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileWeb.Migrations
{
	/// <inheritdoc />
	public partial class InitialOrder : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "NumberOfBehindCamera",
				table: "Specifications",
				newName: "NumberOfBehindCamera");

			migrationBuilder.CreateTable(
				name: "Orders",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Status = table.Column<int>(type: "int", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
					PaymentMethod = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Orders", x => x.Id);
					table.ForeignKey(
						name: "FK_Orders_Users_UserId",
						column: x => x.UserId,
						principalTable: "Users",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "OrderItems",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					OrderId = table.Column<int>(type: "int", nullable: false),
					ProductId = table.Column<int>(type: "int", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false),
					UnitPrice = table.Column<double>(type: "float", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_OrderItems", x => x.Id);
					table.ForeignKey(
						name: "FK_OrderItems_Orders_OrderId",
						column: x => x.OrderId,
						principalTable: "Orders",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_OrderItems_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_OrderItems_OrderId",
				table: "OrderItems",
				column: "OrderId");

			migrationBuilder.CreateIndex(
				name: "IX_OrderItems_ProductId",
				table: "OrderItems",
				column: "ProductId");

			migrationBuilder.CreateIndex(
				name: "IX_Orders_UserId",
				table: "Orders",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "OrderItems");

			migrationBuilder.DropTable(
				name: "Orders");

			migrationBuilder.RenameColumn(
				name: "NumberOfBehindCamera",
				table: "Specifications",
				newName: "NumberOfBehindCamera");
		}
	}
}
