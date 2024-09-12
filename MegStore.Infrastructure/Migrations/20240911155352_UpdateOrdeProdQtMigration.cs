using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrdeProdQtMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemQuantiy",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemQuantiy",
                table: "Products");
        }
    }
}
