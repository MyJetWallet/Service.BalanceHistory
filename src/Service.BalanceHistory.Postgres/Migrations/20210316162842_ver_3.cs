using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class ver_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "balancehistory",
                table: "operation_info",
                type: "integer",
                nullable: false,
                defaultValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "balancehistory",
                table: "operation_info");
        }
    }
}
