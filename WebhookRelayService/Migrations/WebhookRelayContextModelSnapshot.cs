﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebhookRelayService;

#nullable disable

namespace WebhookRelayService.Migrations
{
    [DbContext(typeof(WebhookRelayContext))]
    partial class WebhookRelayContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebhookRelayService.Models.WebhookUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("NotificationEndpoint")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WebhookId")
                        .HasColumnType("integer");

                    b.Property<string>("WebhookSecret")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WebhookUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
