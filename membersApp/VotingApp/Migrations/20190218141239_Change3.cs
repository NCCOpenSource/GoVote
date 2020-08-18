using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingApp.Migrations
{
    public partial class Change3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembersSeats",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberAzureId = table.Column<string>(nullable: false),
                    Seat = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersSeats", x => x.id);
                    table.ForeignKey(
                        name: "FK_MembersSeats_Members_MemberAzureId",
                        column: x => x.MemberAzureId,
                        principalTable: "Members",
                        principalColumn: "AzureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembersSeats_MemberAzureId",
                table: "MembersSeats",
                column: "MemberAzureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembersSeats");
        }
    }
}
