﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class ver_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operation_info",
                schema: "balancehistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", maxLength: 128, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ChangeType = table.Column<int>(type: "integer", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApplicationEnvInfo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation_info", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_OperationId",
                schema: "balancehistory",
                table: "balance_history",
                column: "OperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operation_info",
                schema: "balancehistory");

            migrationBuilder.DropIndex(
                name: "IX_balance_history_OperationId",
                schema: "balancehistory",
                table: "balance_history");
        }
    }
}
