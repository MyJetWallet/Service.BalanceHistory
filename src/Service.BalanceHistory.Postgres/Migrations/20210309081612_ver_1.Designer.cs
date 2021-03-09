﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.BalanceHistory.Postgres;

namespace Service.BalanceHistory.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20210309081612_ver_1")]
    partial class ver_1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("balancehistory")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Service.BalanceHistory.Postgres.BalanceHistoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("AmountBalance")
                        .HasColumnType("double precision");

                    b.Property<double>("AmountReserve")
                        .HasColumnType("double precision");

                    b.Property<double>("AvailableBalance")
                        .HasColumnType("double precision");

                    b.Property<string>("BrokerId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ClientId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("EventType")
                        .HasColumnType("text");

                    b.Property<bool>("IsBalanceChanged")
                        .HasColumnType("boolean");

                    b.Property<double>("NewBalance")
                        .HasColumnType("double precision");

                    b.Property<double>("NewReserve")
                        .HasColumnType("double precision");

                    b.Property<double>("OldBalance")
                        .HasColumnType("double precision");

                    b.Property<double>("OldReserve")
                        .HasColumnType("double precision");

                    b.Property<string>("OperationId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<long>("SequenceId")
                        .HasColumnType("bigint");

                    b.Property<string>("Symbol")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("WalletId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.HasIndex("OperationId");

                    b.HasIndex("SequenceId");

                    b.HasIndex("WalletId");

                    b.HasIndex("WalletId", "SequenceId");

                    b.HasIndex("WalletId", "Symbol");

                    b.HasIndex("SequenceId", "WalletId", "Symbol")
                        .IsUnique();

                    b.HasIndex("WalletId", "Symbol", "SequenceId");

                    b.ToTable("balance_history");
                });

            modelBuilder.Entity("Service.BalanceHistory.Postgres.WalletBalanceUpdateOperationInfoEntity", b =>
                {
                    b.Property<string>("OperationId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ApplicationEnvInfo")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("ApplicationName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("ChangeType")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Changer")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.HasKey("OperationId");

                    b.ToTable("operation_info");
                });
#pragma warning restore 612, 618
        }
    }
}
