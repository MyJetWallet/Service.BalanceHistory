using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class ver_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TxId",
                schema: "balancehistory",
                table: "operation_info",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TxId",
                schema: "balancehistory",
                table: "operation_info");
        }
    }
}
