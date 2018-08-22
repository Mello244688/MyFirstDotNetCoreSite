
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<DraftPlayer>().HasKey(t => new { t.DraftId, t.PlayerId });
            builder.Entity<TeamPlayer>().HasKey(t => new {t.TeamId, t.PlayerId});

            //team has one user with many teams
            builder.Entity<Team>()
                .HasOne(u => u.User)
                .WithMany(t => t.UserTeams)
                .IsRequired();

            //team has one draft with many teams
            builder.Entity<Team>()
                .HasOne(d => d.Draft)
                .WithMany(t => t.TeamsInDraft)
                .HasForeignKey(t => t.DraftId)
                .IsRequired();

            builder.Entity<DraftPlayer>()
                .HasOne(dp => dp.Draft)
                .WithMany(d => d.AvailablePlayers)
                .HasForeignKey(dp => dp.DraftId);

            builder.Entity<DraftPlayer>()
                .HasOne(dp => dp.Player)
                .WithMany(p => p.DraftPlayer)
                .HasForeignKey(dp => dp.PlayerId);

            builder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Team)
                .WithMany(t => t.LineUp)
                .HasForeignKey(tp => tp.TeamId);

            builder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Player)
                .WithMany(p => p.TeamPlayer)
                .HasForeignKey(tp => tp.PlayerId);
            
            base.OnModelCreating(builder);
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
