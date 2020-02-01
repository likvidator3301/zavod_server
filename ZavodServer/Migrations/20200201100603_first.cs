using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Models;
using ZavodServer.Models;

namespace ZavodServer.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Position = table.Column<Vector3>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultBuildings",
                columns: table => new
                {
                    Type = table.Column<int>(nullable: false),
                    BuildingDto = table.Column<BuildingDb>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultBuildings", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "DefaultUnits",
                columns: table => new
                {
                    Type = table.Column<int>(nullable: false),
                    UnitDto = table.Column<UnitDb>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultUnits", x => x.Type);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    Units = table.Column<List<Guid>>(nullable: true),
                    Buildings = table.Column<List<Guid>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "DefaultBuildings");

            migrationBuilder.DropTable(
                name: "DefaultUnits");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
