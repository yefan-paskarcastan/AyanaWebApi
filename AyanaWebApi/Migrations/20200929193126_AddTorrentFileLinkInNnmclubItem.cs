using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddTorrentFileLinkInNnmclubItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Torrent",
                table: "NnmclubItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Torrent",
                table: "NnmclubItems");
        }
    }
}
