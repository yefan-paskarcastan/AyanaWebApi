using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddTorrentSoftPostInputsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TorrentSoftAddPostInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TorrentSoftPostId = table.Column<int>(nullable: false),
                    BaseAddress = table.Column<string>(nullable: true),
                    AddPostAddress = table.Column<string>(nullable: true),
                    UploadFileAddress = table.Column<string>(nullable: true),
                    UserHashHttpHeaderName = table.Column<string>(nullable: true),
                    UserHashFindVarName = table.Column<string>(nullable: true),
                    UserHashExStringCount = table.Column<int>(nullable: false),
                    UserHashLength = table.Column<int>(nullable: false),
                    AddPostFormPosterHeader = table.Column<string>(nullable: true),
                    AddPostFormNameHeader = table.Column<string>(nullable: true),
                    AddPostFormDescriptionHeader = table.Column<string>(nullable: true),
                    AddPostFormScreenshotTemplateStartHeader = table.Column<string>(nullable: true),
                    AddPostFormScreenshotTemplateEndHeader = table.Column<string>(nullable: true),
                    AddPostFormMaxCountScreenshots = table.Column<int>(nullable: false),
                    AddPostFormFileHeader = table.Column<string>(nullable: true),
                    FormDataId = table.Column<string>(nullable: true),
                    PosterUploadQueryStringId = table.Column<string>(nullable: true),
                    TorrentUploadQueryStringId = table.Column<string>(nullable: true),
                    AuthDataId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftAddPostInputs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TorrentSoftAddPostInputs");
        }
    }
}
