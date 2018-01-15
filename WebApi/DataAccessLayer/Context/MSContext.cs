using Common.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context
{
    public class MSContext : DbContext
    {
        public MSContext(DbContextOptions<MSContext> options) : base(options)
        { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TokenEntity> Tokens { get; set; }
        public DbSet<UserTeamEntity> UserTeams { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<LobbieEntity> Lobbies { get; set; }
        //public DbSet<ConfirmationPasswordEntity> ConfirmationPasswords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTeamEntity>()
           .HasKey(t => new { t.UserId, t.TeamId });

            modelBuilder.Entity<UserTeamEntity>()
                .HasOne(sc => sc.User)
                .WithMany(s => s.UserTeams)
                .HasForeignKey(sc => sc.UserId);

            modelBuilder.Entity<UserTeamEntity>()
                .HasOne(sc => sc.Team)
                .WithMany(c => c.UserTeams)
                .HasForeignKey(sc => sc.TeamId);
        }
    }
}
