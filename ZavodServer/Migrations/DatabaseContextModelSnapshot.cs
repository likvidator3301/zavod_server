﻿// <auto-generated />
using System;
//using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ZavodServer;
using ZavodServer.Models;

namespace ZavodServer.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ZavodServer.Models.BuildingDb", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Vector3>("Position")
                        .HasColumnType("jsonb");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("ZavodServer.Models.DefaultBuildingDb", b =>
                {
                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<BuildingDb>("BuildingDto")
                        .HasColumnType("jsonb");

                    b.HasKey("Type");

                    b.ToTable("DefaultBuildings");
                });

            modelBuilder.Entity("ZavodServer.Models.DefaultUnitDb", b =>
                {
                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<UnitDb>("UnitDto")
                        .HasColumnType("jsonb");

                    b.HasKey("Type");

                    b.ToTable("DefaultUnits");
                });

            modelBuilder.Entity("ZavodServer.Models.UnitDb", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<float>("AttackDamage")
                        .HasColumnType("real");

                    b.Property<float>("AttackDelay")
                        .HasColumnType("real");

                    b.Property<float>("AttackRange")
                        .HasColumnType("real");

                    b.Property<float>("CurrentHp")
                        .HasColumnType("real");

                    b.Property<float>("Defense")
                        .HasColumnType("real");

                    b.Property<float>("LastAttackTime")
                        .HasColumnType("real");

                    b.Property<float>("MaxHp")
                        .HasColumnType("real");

                    b.Property<float>("MoveSpeed")
                        .HasColumnType("real");

                    b.Property<Vector3>("Position")
                        .HasColumnType("jsonb");

                    b.Property<Vector3>("Rotation")
                        .HasColumnType("jsonb");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });
#pragma warning restore 612, 618
        }
    }
}
