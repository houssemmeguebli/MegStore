using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiscountProductMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "discountPercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discountPercentage",
                table: "Products");
        }
    }
}
