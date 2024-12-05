using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CKMS.AdminUserService.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsActive",
                table: "Kitchens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Kitchens");
        }
    }
}
