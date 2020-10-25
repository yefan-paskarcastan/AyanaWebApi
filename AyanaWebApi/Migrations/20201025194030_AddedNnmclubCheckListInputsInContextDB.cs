using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddedNnmclubCheckListInputsInContextDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NnmclubCheckListInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UriList = table.Column<string>(nullable: true),
                    UriListIncrement = table.Column<int>(nullable: false),
                    UriListCount = table.Column<int>(nullable: false),
                    AuthPage = table.Column<string>(nullable: true),
                    AuthParam = table.Column<string>(nullable: true),
                    XPathDate = table.Column<string>(nullable: true),
                    DateTimeFormat = table.Column<string>(nullable: true),
                    XPathName = table.Column<string>(nullable: true),
                    ProxySocks5Addr = table.Column<string>(nullable: true),
                    ProxySocks5Port = table.Column<int>(nullable: false),
                    ProxyActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NnmclubCheckListInputs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NnmclubCheckListInputs");
        }
    }
}
