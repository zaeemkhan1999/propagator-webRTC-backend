using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apsy.App.Propagator.Infrastructure.Migrations
{
    public partial class IconLayoutType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IconLayoutType",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconLayoutType",
                table: "Post");
        }
    }
}
