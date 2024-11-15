﻿// <auto-generated />
using System;
using ItbApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ItbApi.Migrations
{
    [DbContext(typeof(ItbContext))]
    partial class ItbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ItbApi.Beverage", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("Accepted")
                        .HasColumnType("bit");

                    b.Property<decimal>("AlcoholPercentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BreweryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("IBU")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ImageUrlMed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrlSm")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IpAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LableMed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LableSm")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MajorGroup")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<decimal>("UtappedRating")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("BreweryId");

                    b.HasIndex("MajorGroup");

                    b.HasIndex("Type");

                    b.ToTable("Beverages");
                });

            modelBuilder.Entity("ItbApi.BeverageGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("BeverageGroups");
                });

            modelBuilder.Entity("ItbApi.BeverageReview", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("BeverageId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalScore")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("BeverageReviews");
                });

            modelBuilder.Entity("ItbApi.BeverageType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("BeverageTypes");
                });

            modelBuilder.Entity("ItbApi.Brewery", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Facebook")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Logo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Twitter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WebPage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Breweries");
                });

            modelBuilder.Entity("ItbApi.ReviewPart", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("BeverageReviewId")
                        .HasColumnType("bigint");

                    b.Property<int>("ReviewTypeId")
                        .HasColumnType("int");

                    b.Property<decimal>("Score")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("BeverageReviewId");

                    b.HasIndex("ReviewTypeId");

                    b.ToTable("ReviewParts");
                });

            modelBuilder.Entity("ItbApi.ReviewType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Abbr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Accepted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ReviewTypes");
                });

            modelBuilder.Entity("ItbApi.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BlindRevealCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Pin")
                        .HasColumnType("int");

                    b.Property<int>("RoomType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("ItbApi.Room2Beverage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BeverageId")
                        .HasColumnType("int");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<int>("FinalScore")
                        .HasColumnType("int");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("bit");

                    b.Property<bool>("ReviewFinished")
                        .HasColumnType("bit");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BeverageId");

                    b.HasIndex("RoomId");

                    b.ToTable("Room2Beverages");
                });

            modelBuilder.Entity("ItbApi.Room2ReviewType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<int>("MaxValue")
                        .HasColumnType("int");

                    b.Property<int>("ReviewTypeId")
                        .HasColumnType("int");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReviewTypeId");

                    b.HasIndex("RoomId");

                    b.ToTable("Room2ReviewTypes");
                });

            modelBuilder.Entity("ItbApi.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ItbApi.Beverage", b =>
                {
                    b.HasOne("ItbApi.Brewery", "Brewery")
                        .WithMany("Beverages")
                        .HasForeignKey("BreweryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ItbApi.BeverageGroup", "BeverageGroup")
                        .WithMany("Beverages")
                        .HasForeignKey("MajorGroup")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ItbApi.BeverageType", "BeverageType")
                        .WithMany("Beverages")
                        .HasForeignKey("Type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BeverageGroup");

                    b.Navigation("BeverageType");

                    b.Navigation("Brewery");
                });

            modelBuilder.Entity("ItbApi.BeverageReview", b =>
                {
                    b.HasOne("ItbApi.Room", "Room")
                        .WithMany("BeverageReviews")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ItbApi.User", "User")
                        .WithMany("BeverageReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ItbApi.ReviewPart", b =>
                {
                    b.HasOne("ItbApi.BeverageReview", "BeverageReview")
                        .WithMany("ReviewParts")
                        .HasForeignKey("BeverageReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ItbApi.ReviewType", "ReviewType")
                        .WithMany("ReviewParts")
                        .HasForeignKey("ReviewTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BeverageReview");

                    b.Navigation("ReviewType");
                });

            modelBuilder.Entity("ItbApi.Room2Beverage", b =>
                {
                    b.HasOne("ItbApi.Beverage", "Beverage")
                        .WithMany("Room2Beverages")
                        .HasForeignKey("BeverageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ItbApi.Room", "Room")
                        .WithMany("Room2Beverages")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beverage");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("ItbApi.Room2ReviewType", b =>
                {
                    b.HasOne("ItbApi.ReviewType", "ReviewType")
                        .WithMany("Room2ReviewTypes")
                        .HasForeignKey("ReviewTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ItbApi.Room", "Room")
                        .WithMany("Room2ReviewTypes")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReviewType");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("ItbApi.User", b =>
                {
                    b.HasOne("ItbApi.Room", "Room")
                        .WithMany("Users")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("ItbApi.Beverage", b =>
                {
                    b.Navigation("Room2Beverages");
                });

            modelBuilder.Entity("ItbApi.BeverageGroup", b =>
                {
                    b.Navigation("Beverages");
                });

            modelBuilder.Entity("ItbApi.BeverageReview", b =>
                {
                    b.Navigation("ReviewParts");
                });

            modelBuilder.Entity("ItbApi.BeverageType", b =>
                {
                    b.Navigation("Beverages");
                });

            modelBuilder.Entity("ItbApi.Brewery", b =>
                {
                    b.Navigation("Beverages");
                });

            modelBuilder.Entity("ItbApi.ReviewType", b =>
                {
                    b.Navigation("ReviewParts");

                    b.Navigation("Room2ReviewTypes");
                });

            modelBuilder.Entity("ItbApi.Room", b =>
                {
                    b.Navigation("BeverageReviews");

                    b.Navigation("Room2Beverages");

                    b.Navigation("Room2ReviewTypes");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("ItbApi.User", b =>
                {
                    b.Navigation("BeverageReviews");
                });
#pragma warning restore 612, 618
        }
    }
}
