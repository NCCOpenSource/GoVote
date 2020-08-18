using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingApp.Migrations
{
    public partial class Change1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AzureID",
                table: "Attendeess",
                newName: "MemberAzureId");

            migrationBuilder.AlterColumn<string>(
                name: "MemberAzureId",
                table: "Attendeess",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Attendeess_MemberAzureId",
                table: "Attendeess",
                column: "MemberAzureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendeess_Members_MemberAzureId",
                table: "Attendeess",
                column: "MemberAzureId",
                principalTable: "Members",
                principalColumn: "AzureId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendeess_Members_MemberAzureId",
                table: "Attendeess");

            migrationBuilder.DropIndex(
                name: "IX_Attendeess_MemberAzureId",
                table: "Attendeess");

            migrationBuilder.RenameColumn(
                name: "MemberAzureId",
                table: "Attendeess",
                newName: "AzureID");

            migrationBuilder.AlterColumn<string>(
                name: "AzureID",
                table: "Attendeess",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
