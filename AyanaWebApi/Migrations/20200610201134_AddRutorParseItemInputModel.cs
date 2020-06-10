using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddRutorParseItemInputModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RutorParseItemInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UriItem = table.Column<string>(nullable: true),
                    ProxySocks5Addr = table.Column<string>(nullable: true),
                    ProxySocks5Port = table.Column<int>(nullable: false),
                    XPathExprDescription = table.Column<string>(nullable: true),
                    XPathExprSpoiler = table.Column<string>(nullable: true),
                    XPathExprImgs = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutorParseItemInputs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RutorParseItemInputs");
        }
    }
}
