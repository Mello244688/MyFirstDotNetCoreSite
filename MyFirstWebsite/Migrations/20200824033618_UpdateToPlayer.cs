using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFirstWebsite.Migrations
{
    public partial class UpdateToPlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerUrl",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerUrl",
                table: "Players");
        }
    }
}
