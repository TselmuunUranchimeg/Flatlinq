using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flatlinq.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoldMember",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsGoldMember",
                table: "Landlords");

            migrationBuilder.AddColumn<bool>(
                name: "IsGoldMember",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoldMember",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsGoldMember",
                table: "Tenants",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoldMember",
                table: "Landlords",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
