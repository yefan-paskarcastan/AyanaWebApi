using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddedNnmclubParseItemInputsInContextDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NnmclubParseItemInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UriItem = table.Column<string>(nullable: true),
                    AuthPage = table.Column<string>(nullable: true),
                    AuthParam = table.Column<string>(nullable: true),
                    ProxySocks5Addr = table.Column<string>(nullable: true),
                    ProxySocks5Port = table.Column<int>(nullable: false),
                    ProxyActive = table.Column<bool>(nullable: false),
                    XPathDescription = table.Column<string>(nullable: true),
                    XPathSpoiler = table.Column<string>(nullable: true),
                    XPathPoster = table.Column<string>(nullable: true),
                    XPathImgs = table.Column<string>(nullable: true),
                    XPathTorrent = table.Column<string>(nullable: true),
                    XPathTrash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnmclubParseItemInputs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnmclubParseItemInputs");
        }
    }
}
