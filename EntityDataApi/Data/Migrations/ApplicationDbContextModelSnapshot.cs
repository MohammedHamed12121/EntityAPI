﻿// <auto-generated />
using System;
using EntityDataApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EntityDataApi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EntityDataApi.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AddressLine")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("EntityDataApi.Models.Date", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DateType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateValue")
                        .HasColumnType("datetime2");

                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.ToTable("Dates");
                });

            modelBuilder.Entity("EntityDataApi.Models.Entity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Deceased")
                        .HasColumnType("bit");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Entities");
                });

            modelBuilder.Entity("EntityDataApi.Models.Name", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EntityId");

                    b.ToTable("Names");
                });

            modelBuilder.Entity("EntityDataApi.Models.Address", b =>
                {
                    b.HasOne("EntityDataApi.Models.Entity", "Entity")
                        .WithMany("Addresses")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("EntityDataApi.Models.Date", b =>
                {
                    b.HasOne("EntityDataApi.Models.Entity", "Entity")
                        .WithMany("Dates")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("EntityDataApi.Models.Name", b =>
                {
                    b.HasOne("EntityDataApi.Models.Entity", "Entity")
                        .WithMany("Names")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("EntityDataApi.Models.Entity", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Dates");

                    b.Navigation("Names");
                });
#pragma warning restore 612, 618
        }
    }
}
