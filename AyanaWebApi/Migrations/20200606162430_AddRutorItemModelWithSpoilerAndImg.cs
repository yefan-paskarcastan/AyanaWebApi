using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddRutorItemModelWithSpoilerAndImg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RutorItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RutorListItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutorItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutorItems_RutorListItems_RutorListItemId",
                        column: x => x.RutorListItemId,
                        principalTable: "RutorListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RutorItemImgs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    ParentUrl = table.Column<string>(nullable: true),
                    ChildUrl = table.Column<string>(nullable: true),
                    RutorItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutorItemImgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutorItemImgs_RutorItems_RutorItemId",
                        column: x => x.RutorItemId,
                        principalTable: "RutorItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RutorItemSpoilers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Header = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    RutorItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutorItemSpoilers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutorItemSpoilers_RutorItems_RutorItemId",
                        column: x => x.RutorItemId,
                        principalTable: "RutorItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RutorItemImgs_RutorItemId",
                table: "RutorItemImgs",
                column: "RutorItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RutorItems_RutorListItemId",
                table: "RutorItems",
                column: "RutorListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RutorItemSpoilers_RutorItemId",
                table: "RutorItemSpoilers",
                column: "RutorItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RutorItemImgs");

            migrationBuilder.DropTable(
                name: "RutorItemSpoilers");

            migrationBuilder.DropTable(
                name: "RutorItems");
        }
    }
}
