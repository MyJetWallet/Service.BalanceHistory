using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class CashInOutandFee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeeAsset",
                schema: "balancehistory",
                table: "trade_history",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FeeVolume",
                schema: "balancehistory",
                table: "trade_history",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "cash_in_out_history",
                schema: "balancehistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WalletId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Volume = table.Column<double>(type: "double precision", nullable: false),
                    FeeVolume = table.Column<double>(type: "double precision", nullable: false),
                    Asset = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    FeeAsset = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SequenceId = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: true),
                    OperationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    OperationType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_in_out_history", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_OperationId",
                schema: "balancehistory",
                table: "cash_in_out_history",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_SequenceId",
                schema: "balancehistory",
                table: "cash_in_out_history",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_SequenceId_WalletId_Asset",
                schema: "balancehistory",
                table: "cash_in_out_history",
                columns: new[] { "SequenceId", "WalletId", "Asset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_WalletId",
                schema: "balancehistory",
                table: "cash_in_out_history",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_WalletId_Asset",
                schema: "balancehistory",
                table: "cash_in_out_history",
                columns: new[] { "WalletId", "Asset" });

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_WalletId_Asset_SequenceId",
                schema: "balancehistory",
                table: "cash_in_out_history",
                columns: new[] { "WalletId", "Asset", "SequenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_cash_in_out_history_WalletId_SequenceId",
                schema: "balancehistory",
                table: "cash_in_out_history",
                columns: new[] { "WalletId", "SequenceId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_in_out_history",
                schema: "balancehistory");

            migrationBuilder.DropColumn(
                name: "FeeAsset",
                schema: "balancehistory",
                table: "trade_history");

            migrationBuilder.DropColumn(
                name: "FeeVolume",
                schema: "balancehistory",
                table: "trade_history");
        }
    }
}
