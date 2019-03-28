﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SeaBattle.Server.Models;

namespace SeaBattle.Server.Migrations
{
    using Dal;

    [DbContext(typeof(ApplicationContext))]
    [Migration("20190327212143_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SeaBattle.Server.Dal.Entities.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<byte[]>("Strategy");

                    b.Property<int>("TelegramId");

                    b.HasKey("Id");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("SeaBattle.Server.Dal.Entities.PlayedGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Rated");

                    b.Property<string>("Result")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.ToTable("PlayedGames");
                });

            modelBuilder.Entity("SeaBattle.Server.Dal.Entities.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GamesPlayed");

                    b.Property<int>("Losses");

                    b.Property<int>("ParticipantId");

                    b.Property<double>("Rating");

                    b.Property<int>("Wins");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId")
                        .IsUnique();

                    b.ToTable("Statistic");
                });

            modelBuilder.Entity("SeaBattle.Server.Dal.Entities.StrategySource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("LoadDate");

                    b.Property<int>("ParticipantId");

                    b.Property<byte[]>("Sources");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("StrategySources");
                });

            modelBuilder.Entity("SeaBattle.Server.Dal.Entities.Statistic", b =>
                {
                    b.HasOne("SeaBattle.Server.Dal.Entities.Participant", "Participant")
                        .WithOne("Statistic")
                        .HasForeignKey("SeaBattle.Server.Dal.Entities.Statistic", "ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SeaBattle.Server.Dal.Entities.StrategySource", b =>
                {
                    b.HasOne("SeaBattle.Server.Dal.Entities.Participant", "Participant")
                        .WithMany("StrategySources")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
