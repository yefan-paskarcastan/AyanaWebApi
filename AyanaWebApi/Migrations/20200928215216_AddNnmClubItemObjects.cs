using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddNnmClubItemObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NnmclubItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Poster = table.Column<string>(nullable: true),
                    NnmclubListItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnmclubItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NnmclubItems_NnmclubListItems_NnmclubListItemId",
                        column: x => x.NnmclubListItemId,
                        principalTable: "NnmclubListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NnmclubItemImgs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    ImgUri = table.Column<string>(nullable: true),
                    NnmclubItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnmclubItemImgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NnmclubItemImgs_NnmclubItems_NnmclubItemId",
                        column: x => x.NnmclubItemId,
                        principalTable: "NnmclubItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NnmclubItemSpoilers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Header = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    NnmclubItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnmclubItemSpoilers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NnmclubItemSpoilers_NnmclubItems_NnmclubItemId",
                        column: x => x.NnmclubItemId,
                        principalTable: "NnmclubItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NnmclubItemImgs_NnmclubItemId",
                table: "NnmclubItemImgs",
                column: "NnmclubItemId");

            migrationBuilder.CreateIndex(
                name: "IX_NnmclubItems_NnmclubListItemId",
                table: "NnmclubItems",
                column: "NnmclubListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_NnmclubItemSpoilers_NnmclubItemId",
                table: "NnmclubItemSpoilers",
                column: "NnmclubItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnmclubItemImgs");

            migrationBuilder.DropTable(
                name: "NnmclubItemSpoilers");

            migrationBuilder.DropTable(
                name: "NnmclubItems");
        }
    }
}
