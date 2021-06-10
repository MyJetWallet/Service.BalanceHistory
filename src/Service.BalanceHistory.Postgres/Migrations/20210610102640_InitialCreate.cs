using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "balancehistory");

            migrationBuilder.CreateTable(
                name: "balance_history",
                schema: "balancehistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WalletId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Symbol = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AmountBalance = table.Column<double>(type: "double precision", nullable: false),
                    AmountReserve = table.Column<double>(type: "double precision", nullable: false),
                    OldBalance = table.Column<double>(type: "double precision", nullable: false),
                    NewBalance = table.Column<double>(type: "double precision", nullable: false),
                    OldReserve = table.Column<double>(type: "double precision", nullable: false),
                    NewReserve = table.Column<double>(type: "double precision", nullable: false),
                    SequenceId = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: true),
                    OperationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    AvailableBalance = table.Column<double>(type: "double precision", nullable: false),
                    IsBalanceChanged = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balance_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "operation_info",
                schema: "balancehistory",
                columns: table => new
                {
                    OperationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ApplicationName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApplicationEnvInfo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Changer = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    TxId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    WithdrawalAddress = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation_info", x => x.OperationId);
                });

            migrationBuilder.CreateTable(
                name: "operation_info_rawdata",
                schema: "balancehistory",
                columns: table => new
                {
                    OperationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RawData = table.Column<string>(type: "character varying(5120)", maxLength: 5120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation_info_rawdata", x => x.OperationId);
                });

            migrationBuilder.CreateTable(
                name: "swap_history",
                schema: "balancehistory",
                columns: table => new
                {
                    OperationId = table.Column<string>(type: "text", nullable: false),
                    WalletId = table.Column<string>(type: "text", nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: true),
                    FromAsset = table.Column<string>(type: "text", nullable: true),
                    ToAsset = table.Column<string>(type: "text", nullable: true),
                    FromVolume = table.Column<string>(type: "text", nullable: true),
                    ToVolume = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swap_history", x => new { x.OperationId, x.WalletId });
                });

            migrationBuilder.CreateTable(
                name: "trade_history",
                schema: "balancehistory",
                columns: table => new
                {
                    TradeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrokerId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WalletId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    InstrumentSymbol = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    BaseVolume = table.Column<double>(type: "double precision", nullable: false),
                    QuoteVolume = table.Column<double>(type: "double precision", nullable: false),
                    OrderId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    OrderVolume = table.Column<double>(type: "double precision", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TradeUId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Side = table.Column<int>(type: "integer", nullable: false),
                    SequenceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trade_history", x => x.TradeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_OperationId",
                schema: "balancehistory",
                table: "balance_history",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_SequenceId",
                schema: "balancehistory",
                table: "balance_history",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_SequenceId_WalletId_Symbol",
                schema: "balancehistory",
                table: "balance_history",
                columns: new[] { "SequenceId", "WalletId", "Symbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_WalletId",
                schema: "balancehistory",
                table: "balance_history",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_WalletId_SequenceId",
                schema: "balancehistory",
                table: "balance_history",
                columns: new[] { "WalletId", "SequenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_WalletId_Symbol",
                schema: "balancehistory",
                table: "balance_history",
                columns: new[] { "WalletId", "Symbol" });

            migrationBuilder.CreateIndex(
                name: "IX_balance_history_WalletId_Symbol_SequenceId",
                schema: "balancehistory",
                table: "balance_history",
                columns: new[] { "WalletId", "Symbol", "SequenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_swap_history_OperationId_WalletId",
                schema: "balancehistory",
                table: "swap_history",
                columns: new[] { "OperationId", "WalletId" });

            migrationBuilder.CreateIndex(
                name: "IX_swap_history_SequenceNumber",
                schema: "balancehistory",
                table: "swap_history",
                column: "SequenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_trade_history_SequenceId",
                schema: "balancehistory",
                table: "trade_history",
                column: "SequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_trade_history_TradeUId",
                schema: "balancehistory",
                table: "trade_history",
                column: "TradeUId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trade_history_WalletId",
                schema: "balancehistory",
                table: "trade_history",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_trade_history_WalletId_InstrumentSymbol",
                schema: "balancehistory",
                table: "trade_history",
                columns: new[] { "WalletId", "InstrumentSymbol" });

            migrationBuilder.CreateIndex(
                name: "IX_trade_history_WalletId_InstrumentSymbol_SequenceId",
                schema: "balancehistory",
                table: "trade_history",
                columns: new[] { "WalletId", "InstrumentSymbol", "SequenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_trade_history_WalletId_SequenceId",
                schema: "balancehistory",
                table: "trade_history",
                columns: new[] { "WalletId", "SequenceId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "balance_history",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "operation_info",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "operation_info_rawdata",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "swap_history",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "trade_history",
                schema: "balancehistory");
        }
    }
}
