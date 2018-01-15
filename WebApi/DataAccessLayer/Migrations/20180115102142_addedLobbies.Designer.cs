﻿// <auto-generated />
using Common.Enums;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(MSContext))]
    [Migration("20180115102142_addedLobbies")]
    partial class addedLobbies
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Common.Entity.LobbieEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AmountPlayers");

                    b.Property<double>("Bet");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("ExpectingSeconds");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("Map");

                    b.Property<string>("Password");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("Lobbies");
                });

            modelBuilder.Entity("Common.Entity.TeamEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LobbieId");

                    b.HasKey("Id");

                    b.HasIndex("LobbieId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Common.Entity.TokenEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Common.Entity.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Balance");

                    b.Property<string>("Email");

                    b.Property<bool>("IsEmailConfirmed");

                    b.Property<string>("NickName");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<DateTime>("RegistrationDate");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Common.Entity.UserTeamEntity", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("TeamId");

                    b.HasKey("UserId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("UserTeams");
                });

            modelBuilder.Entity("Common.Entity.TeamEntity", b =>
                {
                    b.HasOne("Common.Entity.LobbieEntity", "Lobbie")
                        .WithMany("Teams")
                        .HasForeignKey("LobbieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Common.Entity.TokenEntity", b =>
                {
                    b.HasOne("Common.Entity.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Common.Entity.UserTeamEntity", b =>
                {
                    b.HasOne("Common.Entity.TeamEntity", "Team")
                        .WithMany("UserTeams")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Common.Entity.UserEntity", "User")
                        .WithMany("UserTeams")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
