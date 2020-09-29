using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class BigRefactoringName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverRutorTorrentInputs");

            migrationBuilder.DropTable(
                name: "TorrentSoftPostInputs");

            migrationBuilder.DropTable(
                name: "TorrentSoftPostScreenshot");

            migrationBuilder.DropTable(
                name: "TorrentSoftResults");

            migrationBuilder.DropTable(
                name: "TorrentSoftPosts");

            migrationBuilder.CreateTable(
                name: "DriverRutorToSoftInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    TorrentUri = table.Column<string>(nullable: true),
                    MaxPosterSize = table.Column<int>(nullable: false),
                    ProxySocks5Addr = table.Column<string>(nullable: true),
                    ProxySocks5Port = table.Column<int>(nullable: false),
                    ProxyActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRutorToSoftInputs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftPostInputs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: false),
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
                    table.PrimaryKey("PK_SoftPostInputs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Spoilers = table.Column<string>(nullable: true),
                    PosterImg = table.Column<string>(nullable: true),
                    TorrentFile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftPostImg",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    SoftPostId = table.Column<int>(nullable: false),
                    ImgUri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftPostImg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftPostImg_SoftPosts_SoftPostId",
                        column: x => x.SoftPostId,
                        principalTable: "SoftPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoftResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    SoftPostId = table.Column<int>(nullable: false),
                    SendPostIsSuccess = table.Column<bool>(nullable: false),
                    TorrentFileIsSuccess = table.Column<bool>(nullable: false),
                    PosterIsSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftResults_SoftPosts_SoftPostId",
                        column: x => x.SoftPostId,
                        principalTable: "SoftPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoftPostImg_SoftPostId",
                table: "SoftPostImg",
                column: "SoftPostId");

            migrationBuilder.CreateIndex(
                name: "IX_SoftResults_SoftPostId",
                table: "SoftResults",
                column: "SoftPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverRutorToSoftInputs");

            migrationBuilder.DropTable(
                name: "SoftPostImg");

            migrationBuilder.DropTable(
                name: "SoftPostInputs");

            migrationBuilder.DropTable(
                name: "SoftResults");

            migrationBuilder.DropTable(
                name: "SoftPosts");

            migrationBuilder.CreateTable(
                name: "DriverRutorTorrentInputs",
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
                    table.PrimaryKey("PK_DriverRutorTorrentInputs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TorrentSoftPostInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    AddPostAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPostFormDescriptionHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPostFormFileHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPostFormMaxCountScreenshots = table.Column<int>(type: "int", nullable: false),
                    AddPostFormNameHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPostFormPosterHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPostFormScreenshotTemplateEndHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPostFormScreenshotTemplateStartHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthDataId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormDataId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterUploadQueryStringId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TorrentUploadQueryStringId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadFileAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserHashExStringCount = table.Column<int>(type: "int", nullable: false),
                    UserHashFindVarName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserHashHttpHeaderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserHashLength = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftPostInputs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TorrentSoftPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterImg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Spoilers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TorrentFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TorrentSoftPostScreenshot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScreenUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TorrentSoftPostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftPostScreenshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentSoftPostScreenshot_TorrentSoftPosts_TorrentSoftPostId",
                        column: x => x.TorrentSoftPostId,
                        principalTable: "TorrentSoftPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TorrentSoftResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PosterIsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    SendPostIsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    TorrentFileIsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    TorrentSoftPostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TorrentSoftResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TorrentSoftResults_TorrentSoftPosts_TorrentSoftPostId",
                        column: x => x.TorrentSoftPostId,
                        principalTable: "TorrentSoftPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TorrentSoftPostScreenshot_TorrentSoftPostId",
                table: "TorrentSoftPostScreenshot",
                column: "TorrentSoftPostId");

            migrationBuilder.CreateIndex(
                name: "IX_TorrentSoftResults_TorrentSoftPostId",
                table: "TorrentSoftResults",
                column: "TorrentSoftPostId");
        }
    }
}
