using Microsoft.EntityFrameworkCore.Migrations;

namespace sigreh.Migrations
{
    public partial class ClientEnterTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnterHour",
                table: "Clients",
                newName: "EnterTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnterTime",
                table: "Clients",
                newName: "EnterHour");
        }
    }
}
