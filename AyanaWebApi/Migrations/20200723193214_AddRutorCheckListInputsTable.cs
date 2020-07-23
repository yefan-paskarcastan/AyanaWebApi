using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddRutorCheckListInputsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RutorCheckListInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UriList = table.Column<string>(nullable: true),
                    XPathExprItemDate = table.Column<string>(nullable: true),
                    XPathExprItemUniqNumber = table.Column<string>(nullable: true),
                    XPathParamSplitSeparator = table.Column<string>(nullable: true),
                    XPathParamSplitIndex = table.Column<int>(nullable: false),
                    XPathExprItemName = table.Column<string>(nullable: true),
                    ProxySocks5Addr = table.Column<string>(nullable: true),
                    ProxySocks5Port = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutorCheckListInputs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RutorCheckListInputs");
        }
    }
}
