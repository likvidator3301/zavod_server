using Microsoft.EntityFrameworkCore.Migrations;
using ZavodServer.Models;

namespace ZavodServer.Migrations
{
    public partial class add_map : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Map>(
                name: "GameMap",
                table: "Sessions",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameMap",
                table: "Sessions");
        }
    }
}
