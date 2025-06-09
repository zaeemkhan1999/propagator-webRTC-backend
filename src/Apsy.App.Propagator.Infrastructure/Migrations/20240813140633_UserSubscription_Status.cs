using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apsy.App.Propagator.Infrastructure.Migrations
{
    public partial class UserSubscription_Status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UsersSubscription",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UsersSubscription");
        }
    }
}
