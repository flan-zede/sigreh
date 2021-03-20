using Microsoft.EntityFrameworkCore.Migrations;

namespace sigreh.Migrations
{
    public partial class UserActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Blocked",
                table: "Users",
                newName: "Active");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Users",
                newName: "Blocked");
        }
    }
}
