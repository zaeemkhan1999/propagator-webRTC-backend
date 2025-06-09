using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apsy.App.Propagator.Infrastructure.Migrations
{
    public partial class ResetPasswordRequest_Edit_Connect_To_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ResetPasswordRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordRequest_UserId",
                table: "ResetPasswordRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordRequest_User_UserId",
                table: "ResetPasswordRequest",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordRequest_User_UserId",
                table: "ResetPasswordRequest");

            migrationBuilder.DropIndex(
                name: "IX_ResetPasswordRequest_UserId",
                table: "ResetPasswordRequest");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ResetPasswordRequest");
        }
    }
}
