using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Models;

namespace ZavodServer.Migrations
{
    public partial class first_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Position = table.Column<Vector3>(type: "jsonb", nullable: true),
                    Rotation = table.Column<Vector3>(type: "jsonb", nullable: true),
                    Type = table.Column<int>(nullable: false),
                    AttackDamage = table.Column<float>(nullable: false),
                    AttackDelay = table.Column<float>(nullable: false),
                    AttackRange = table.Column<float>(nullable: false),
                    Defense = table.Column<float>(nullable: false),
                    MoveSpeed = table.Column<float>(nullable: false),
                    MaxHp = table.Column<float>(nullable: false),
                    CurrentHp = table.Column<float>(nullable: false),
                    LastAttackTime = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
