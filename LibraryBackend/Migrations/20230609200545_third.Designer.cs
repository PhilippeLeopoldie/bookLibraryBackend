﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LibraryBackend.Migrations
{
    [DbContext(typeof(MyLibraryContext))]
    [Migration("20230609200545_third")]
    partial class third
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LibraryBackend.Models.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BookId"));

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("BookId");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("LibraryBackend.Models.Opinion", b =>
                {
                    b.Property<int>("OpinionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OpinionId"));

                    b.Property<int?>("BookId")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.Property<int?>("Like")
                        .HasColumnType("integer");

                    b.Property<string>("View")
                        .HasColumnType("text");

                    b.Property<string>("userName")
                        .HasColumnType("text");

                    b.HasKey("OpinionId");

                    b.HasIndex("BookId");

                    b.ToTable("Opinion");
                });

            modelBuilder.Entity("LibraryBackend.Models.Opinion", b =>
                {
                    b.HasOne("LibraryBackend.Models.Book", "Book")
                        .WithMany("Opinions")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("LibraryBackend.Models.Book", b =>
                {
                    b.Navigation("Opinions");
                });
#pragma warning restore 612, 618
        }
    }
}