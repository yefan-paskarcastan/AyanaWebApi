using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddedNlogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthPage",
                table: "DriverToSoftInputs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthParam",
                table: "DriverToSoftInputs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CallSite = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    Level = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(nullable: true),
                    MachineName = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    Thread = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NLogs");

            migrationBuilder.DropColumn(
                name: "AuthPage",
                table: "DriverToSoftInputs");

            migrationBuilder.DropColumn(
                name: "AuthParam",
                table: "DriverToSoftInputs");
        }
    }
}
