using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class ver_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WithdrawalAddress",
                schema: "balancehistory",
                table: "operation_info",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithdrawalAddress",
                schema: "balancehistory",
                table: "operation_info");
        }
    }
}
