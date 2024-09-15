using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class orderItemsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderProduct",
                columns: table => new
                {
                    OrdersorderId = table.Column<long>(type: "bigint", nullable: false),
                    ProductsproductId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => new { x.OrdersorderId, x.ProductsproductId });
                    table.ForeignKey(
                        name: "FK_OrderProduct_Orders_OrdersorderId",
                        column: x => x.OrdersorderId,
                        principalTable: "Orders",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Products_ProductsproductId",
                        column: x => x.ProductsproductId,
                        principalTable: "Products",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductsproductId",
                table: "OrderProduct",
                column: "ProductsproductId");
        }
    }
}
