﻿// <auto-generated />
using System;
using AyanaWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AyanaWebApi.Migrations
{
    [DbContext(typeof(AyDbContext))]
    [Migration("20200718121732_UpdateLogTable")]
    partial class UpdateLogTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Logs");
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
#pragma warning restore 612, 618
        }
    }
}
