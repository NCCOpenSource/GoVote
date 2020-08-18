using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingApp.Migrations
{
    public partial class Change4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "MembersSeats",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveMember",
                table: "Members",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActiveMember",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MembersSeats",
                newName: "id");
        }
    }
}
