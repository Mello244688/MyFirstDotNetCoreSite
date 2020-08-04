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

            builder.Entity<DraftPlayer>().HasKey(t => new { t.DraftId, t.PlayerId });
            builder.Entity<TeamPlayer>().HasKey(t => new {t.TeamId, t.PlayerId});
            builder.Entity<DraftDraftedPlayer>().HasKey(t => new { t.DraftId, t.PlayerId });

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

            builder.Entity<DraftDraftedPlayer>()
                .HasOne(ddp => ddp.Draft)
                .WithMany(d => d.PlayersDrafted)
                .HasForeignKey(pd => pd.DraftId);

            builder.Entity<DraftDraftedPlayer>()
                .HasOne(ddp => ddp.Player)
                .WithMany(p => p.DraftDraftedPlayer)
                .HasForeignKey(fk => fk.PlayerId);

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
