using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BalanceHistory.Postgres.Migrations
{
    public partial class ver_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operation_info_rawdata",
                schema: "balancehistory");
        }
    }
}
