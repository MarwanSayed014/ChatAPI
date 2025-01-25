﻿// <auto-generated />
using System;
using ChatAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250123233853_AttachmentPaths")]
    partial class AttachmentPaths
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatAPI.Models.Friend", b =>
                {
                    b.Property<Guid>("RequestorUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RespondentUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("FriendshipStatus")
                        .HasColumnType("int");

                    b.HasKey("RequestorUserId", "RespondentUserId");

                    b.HasIndex("RespondentUserId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("ChatAPI.Models.GroupChat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CoverImgPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("GroupChats");
                });

            modelBuilder.Entity("ChatAPI.Models.GroupMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.PrimitiveCollection<string>("AttachmentPaths")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GroupChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("GroupChatId");

                    b.ToTable("GroupMessages");
                });

            modelBuilder.Entity("ChatAPI.Models.GroupMessageStatus", b =>
                {
                    b.Property<Guid>("GroupChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MessageStatus")
                        .HasColumnType("int");

                    b.HasKey("GroupChatId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupMessageStatuses");
                });

            modelBuilder.Entity("ChatAPI.Models.PrivateChat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("PrivateChats");
                });

            modelBuilder.Entity("ChatAPI.Models.PrivateMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AttachmentPaths")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("MessageStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("PrivateChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("PrivateChatId");

                    b.ToTable("PrivateMessages");
                });

            modelBuilder.Entity("ChatAPI.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ProfileImgPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChatAPI.Models.UserConnection", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SignalRConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "SignalRConnectionId");

                    b.ToTable("UserConnections");
                });

            modelBuilder.Entity("ChatAPI.Models.UserGroupChat", b =>
                {
                    b.Property<Guid>("GroupChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GroupChatId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGroupChats");
                });

            modelBuilder.Entity("ChatAPI.Models.UserPrivateChat", b =>
                {
                    b.Property<Guid>("PrivateChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PrivateChatId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPrivateChats");
                });

            modelBuilder.Entity("ChatAPI.Models.Friend", b =>
                {
                    b.HasOne("ChatAPI.Models.User", "RequestorUser")
                        .WithMany()
                        .HasForeignKey("RequestorUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatAPI.Models.User", "RespondentUser")
                        .WithMany()
                        .HasForeignKey("RespondentUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RequestorUser");

                    b.Navigation("RespondentUser");
                });

            modelBuilder.Entity("ChatAPI.Models.GroupMessage", b =>
                {
                    b.HasOne("ChatAPI.Models.GroupChat", "GroupChat")
                        .WithMany()
                        .HasForeignKey("GroupChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GroupChat");
                });

            modelBuilder.Entity("ChatAPI.Models.GroupMessageStatus", b =>
                {
                    b.HasOne("ChatAPI.Models.GroupChat", "GroupChat")
                        .WithMany()
                        .HasForeignKey("GroupChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GroupChat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatAPI.Models.PrivateMessage", b =>
                {
                    b.HasOne("ChatAPI.Models.PrivateChat", "PrivateChat")
                        .WithMany()
                        .HasForeignKey("PrivateChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PrivateChat");
                });

            modelBuilder.Entity("ChatAPI.Models.UserConnection", b =>
                {
                    b.HasOne("ChatAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatAPI.Models.UserGroupChat", b =>
                {
                    b.HasOne("ChatAPI.Models.GroupChat", "GroupChat")
                        .WithMany()
                        .HasForeignKey("GroupChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GroupChat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatAPI.Models.UserPrivateChat", b =>
                {
                    b.HasOne("ChatAPI.Models.PrivateChat", "PrivateChat")
                        .WithMany()
                        .HasForeignKey("PrivateChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ChatAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PrivateChat");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
