using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddProxyActiveFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProxyActive",
                table: "RutorParseItemInputs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProxyActive",
                table: "RutorCheckListInputs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProxyActive",
                table: "DriverRutorTorrentInputs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProxyActive",
                table: "RutorParseItemInputs");

            migrationBuilder.DropColumn(
                name: "ProxyActive",
                table: "RutorCheckListInputs");

            migrationBuilder.DropColumn(
                name: "ProxyActive",
                table: "DriverRutorTorrentInputs");
        }
    }
}
