﻿// <auto-generated />
using System;
using AyanaWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AyanaWebApi.Migrations
{
    [DbContext(typeof(AyDbContext))]
    partial class AyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AyanaWebApi.Models.DictionaryValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DictionaryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DictionaryValues");
                });

            modelBuilder.Entity("AyanaWebApi.Models.DriverRutorTorrentInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<int>("MaxPosterSize")
                        .HasColumnType("int");

                    b.Property<bool>("ProxyActive")
                        .HasColumnType("bit");

                    b.Property<string>("ProxySocks5Addr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProxySocks5Port")
                        .HasColumnType("int");

                    b.Property<string>("TorrentUri")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DriverRutorTorrentInputs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.ImghostParsingInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Attr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Def")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ImghostParsingInputs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("ErrorContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExceptionMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MethodName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.NnmclubListItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Added")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Href")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NnmclubListItems");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorCheckListInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ProxyActive")
                        .HasColumnType("bit");

                    b.Property<string>("ProxySocks5Addr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProxySocks5Port")
                        .HasColumnType("int");

                    b.Property<string>("UriList")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPathExprItemDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPathExprItemName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPathExprItemUniqNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("XPathParamSplitIndex")
                        .HasColumnType("int");

                    b.Property<string>("XPathParamSplitSeparator")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RutorCheckListInputs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RutorListItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RutorListItemId");

                    b.ToTable("RutorItems");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorItemImg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChildUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("ParentUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RutorItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RutorItemId");

                    b.ToTable("RutorItemImgs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorItemSpoiler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Header")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RutorItemId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RutorItemId");

                    b.ToTable("RutorItemSpoilers");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorListItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("HrefNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RutorListItems");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorParseItemInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ProxyActive")
                        .HasColumnType("bit");

                    b.Property<string>("ProxySocks5Addr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProxySocks5Port")
                        .HasColumnType("int");

                    b.Property<string>("UriItem")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPathExprDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPathExprImgs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("XPathExprSpoiler")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RutorParseItemInputs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.TorrentSoftPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PosterImg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Spoilers")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TorrentFile")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TorrentSoftPosts");
                });

            modelBuilder.Entity("AyanaWebApi.Models.TorrentSoftPostInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("AddPostAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddPostFormDescriptionHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddPostFormFileHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AddPostFormMaxCountScreenshots")
                        .HasColumnType("int");

                    b.Property<string>("AddPostFormNameHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddPostFormPosterHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddPostFormScreenshotTemplateEndHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddPostFormScreenshotTemplateStartHeader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthDataId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BaseAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormDataId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PosterUploadQueryStringId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TorrentUploadQueryStringId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UploadFileAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserHashExStringCount")
                        .HasColumnType("int");

                    b.Property<string>("UserHashFindVarName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserHashHttpHeaderName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserHashLength")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TorrentSoftPostInputs");
                });

            modelBuilder.Entity("AyanaWebApi.Models.TorrentSoftPostScreenshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("ScreenUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TorrentSoftPostId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TorrentSoftPostId");

                    b.ToTable("TorrentSoftPostScreenshot");
                });

            modelBuilder.Entity("AyanaWebApi.Models.TorrentSoftResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<bool>("PosterIsSuccess")
                        .HasColumnType("bit");

                    b.Property<bool>("SendPostIsSuccess")
                        .HasColumnType("bit");

                    b.Property<bool>("TorrentFileIsSuccess")
                        .HasColumnType("bit");

                    b.Property<int>("TorrentSoftPostId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TorrentSoftPostId");

                    b.ToTable("TorrentSoftResults");
                });

            modelBuilder.Entity("AyanaWebApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorItem", b =>
                {
                    b.HasOne("AyanaWebApi.Models.RutorListItem", "RutorListItem")
                        .WithMany("RutorItems")
                        .HasForeignKey("RutorListItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorItemImg", b =>
                {
                    b.HasOne("AyanaWebApi.Models.RutorItem", "RutorItem")
                        .WithMany("Imgs")
                        .HasForeignKey("RutorItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AyanaWebApi.Models.RutorItemSpoiler", b =>
                {
                    b.HasOne("AyanaWebApi.Models.RutorItem", "RutorItem")
                        .WithMany("Spoilers")
                        .HasForeignKey("RutorItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AyanaWebApi.Models.TorrentSoftPostScreenshot", b =>
                {
                    b.HasOne("AyanaWebApi.Models.TorrentSoftPost", "TorrentSoftPost")
                        .WithMany("Screenshots")
                        .HasForeignKey("TorrentSoftPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AyanaWebApi.Models.TorrentSoftResult", b =>
                {
                    b.HasOne("AyanaWebApi.Models.TorrentSoftPost", "TorrentSoftPost")
                        .WithMany()
                        .HasForeignKey("TorrentSoftPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
