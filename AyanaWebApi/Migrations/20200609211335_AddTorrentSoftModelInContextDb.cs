using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddTorrentSoftModelInContextDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TorrentSoftPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Spoilers = table.Column<string>(nullable: true),
                    PosterImg = table.Column<string>(nullable: true),
                    TorrentFile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TorrentSoftPostScreenshot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    TorrentSoftPostId = table.Column<int>(nullable: false),
                    ScreenUri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftPostScreenshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentSoftPostScreenshot_TorrentSoftPosts_TorrentSoftPostId",
                        column: x => x.TorrentSoftPostId,
                        principalTable: "TorrentSoftPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TorrentSoftPostScreenshot_TorrentSoftPostId",
                table: "TorrentSoftPostScreenshot",
                column: "TorrentSoftPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TorrentSoftPostScreenshot");

            migrationBuilder.DropTable(
                name: "TorrentSoftPosts");
        }
    }
}
