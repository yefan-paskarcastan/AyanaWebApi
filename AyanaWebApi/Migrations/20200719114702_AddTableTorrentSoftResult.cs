using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddTableTorrentSoftResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TorrentSoftResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    TorrentSoftPostId = table.Column<int>(nullable: false),
                    SendPostIsSuccess = table.Column<bool>(nullable: false),
                    TorrentFileIsSuccess = table.Column<bool>(nullable: false),
                    PosterIsSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentSoftResults_TorrentSoftPosts_TorrentSoftPostId",
                        column: x => x.TorrentSoftPostId,
                        principalTable: "TorrentSoftPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TorrentSoftResults_TorrentSoftPostId",
                table: "TorrentSoftResults",
                column: "TorrentSoftPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TorrentSoftResults");
        }
    }
}
