using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class MakeToSoftServiceUniversal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverRutorToSoftInputs");

            migrationBuilder.CreateTable(
                name: "DriverToSoftInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    TorrentUri = table.Column<string>(nullable: true),
                    MaxPosterSize = table.Column<int>(nullable: false),
                    ProxySocks5Addr = table.Column<string>(nullable: true),
                    ProxySocks5Port = table.Column<int>(nullable: false),
                    ProxyActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverToSoftInputs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverToSoftInputs");

            migrationBuilder.CreateTable(
                name: "DriverRutorToSoftInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxPosterSize = table.Column<int>(type: "int", nullable: false),
                    ProxyActive = table.Column<bool>(type: "bit", nullable: false),
                    ProxySocks5Addr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProxySocks5Port = table.Column<int>(type: "int", nullable: false),
                    TorrentUri = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRutorToSoftInputs", x => x.Id);
                });
        }
    }
}
