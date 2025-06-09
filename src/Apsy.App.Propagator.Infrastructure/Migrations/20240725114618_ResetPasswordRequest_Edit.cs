using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apsy.App.Propagator.Infrastructure.Migrations
{
    public partial class ResetPasswordRequest_Edit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoIdUrl",
                table: "ResetPasswordRequest",
                newName: "GovernmentIssuePhotoId");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "ResetPasswordRequest",
                newName: "ProofOfAddress");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ResetPasswordRequest",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmailOrUsername",
                table: "ResetPasswordRequest",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OtherFiles",
                table: "ResetPasswordRequest",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmailOrUsername",
                table: "ResetPasswordRequest");

            migrationBuilder.DropColumn(
                name: "OtherFiles",
                table: "ResetPasswordRequest");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ResetPasswordRequest",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ProofOfAddress",
                table: "ResetPasswordRequest",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "GovernmentIssuePhotoId",
                table: "ResetPasswordRequest",
                newName: "PhotoIdUrl");
        }
    }
}
