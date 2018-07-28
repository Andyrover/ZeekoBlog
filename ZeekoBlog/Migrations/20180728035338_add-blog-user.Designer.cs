﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("20180728035338_add-blog-user")]
    partial class addbloguser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ZeekoBlog.Core.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlogUserId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("LastEdited");

                    b.Property<string>("Summary");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("BlogUserId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("ZeekoBlog.Core.Models.BlogUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName");

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("BlogUser");
                });

            modelBuilder.Entity("ZeekoBlog.Core.Models.Article", b =>
                {
                    b.HasOne("ZeekoBlog.Core.Models.BlogUser", "BlogUser")
                        .WithMany("Articles")
                        .HasForeignKey("BlogUserId");
                });
#pragma warning restore 612, 618
        }
    }
}
