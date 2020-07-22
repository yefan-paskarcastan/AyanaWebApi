using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class DeleteTorrentSoftIdFromPostInputsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TorrentSoftPostId",
                table: "TorrentSoftPostInputs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TorrentSoftPostId",
                table: "TorrentSoftPostInputs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
