using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Models;

namespace ZavodServer.Migrations
{
    public partial class new_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Buildings",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Type = table.Column<int>(),
                    Position = table.Column<Vector3>("jsonb", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Buildings", x => x.Id); });

            migrationBuilder.CreateTable(
                "DefaultBuildings",
                table => new
                {
                    Type = table.Column<int>(),
                    BuildingDto = table.Column<ServerBuildingDto>("jsonb", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_DefaultBuildings", x => x.Type); });

            migrationBuilder.CreateTable(
                "DefaultUnits",
                table => new
                {
                    Type = table.Column<int>(),
                    UnitDto = table.Column<ServerUnitDto>("jsonb", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_DefaultUnits", x => x.Type); });

            migrationBuilder.CreateTable(
                "Units",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Position = table.Column<Vector3>("jsonb", nullable: true),
                    Rotation = table.Column<Vector3>("jsonb", nullable: true),
                    Type = table.Column<int>(),
                    AttackDamage = table.Column<float>(),
                    AttackDelay = table.Column<float>(),
                    AttackRange = table.Column<float>(),
                    Defense = table.Column<float>(),
                    MoveSpeed = table.Column<float>(),
                    MaxHp = table.Column<float>(),
                    CurrentHp = table.Column<float>(),
                    LastAttackTime = table.Column<float>()
                },
                constraints: table => { table.PrimaryKey("PK_Units", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Buildings");

            migrationBuilder.DropTable(
                "DefaultBuildings");

            migrationBuilder.DropTable(
                "DefaultUnits");

            migrationBuilder.DropTable(
                "Units");
        }
    }
}