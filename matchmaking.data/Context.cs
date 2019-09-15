using matchmaking.data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace matchmaking.data
{
    public class MatchMakingContext : DbContext
    {
        public MatchMakingContext()
        {
        }

        public MatchMakingContext(DbContextOptions<MatchMakingContext> options): base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Filename=matchmaking.db", options =>
                {
                    options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                //entity.HasIndex(e => e.Remoteness);
                entity.Property(e => e.DateTimeAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Match>().ToTable("Matchs");
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                //entity.HasIndex(e => e.Remoteness);
                entity.Property(e => e.DateTimeStarted).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
