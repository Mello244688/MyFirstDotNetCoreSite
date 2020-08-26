using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyFirstWebsite.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Draft>()
                .HasOne(d => d.User)
                .WithMany(u => u.Drafts)
                .HasForeignKey(d => d.UserId);

            builder.Entity<Team>()
                .HasOne(t => t.Draft)
                .WithMany(d => d.Teams)
                .HasForeignKey(t => t.DraftId);

            builder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);
            
            base.OnModelCreating(builder);
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
