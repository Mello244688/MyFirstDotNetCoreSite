using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFirstWebsite.Migrations
{
    public partial class updatePlayerRank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rank",
                table: "Players",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rank",
                table: "Players",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
