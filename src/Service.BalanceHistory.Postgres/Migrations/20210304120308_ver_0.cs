using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class ver_0 : Migration
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
                    ChangeType = table.Column<int>(type: "integer", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApplicationEnvInfo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation_info", x => x.OperationId);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "balance_history",
                schema: "balancehistory");

            migrationBuilder.DropTable(
                name: "operation_info",
                schema: "balancehistory");
        }
    }
}
