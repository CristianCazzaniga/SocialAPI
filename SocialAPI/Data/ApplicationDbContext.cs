using SocialAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SocialAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<LocalUser> LocalUsers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Annuncio> Annunci { get; set; }
        public DbSet<Commento> Commenti { get; set; }
        public DbSet<Messaggio> Messaggi { get; set; }
        public DbSet<Storia> Storie { get; set; }
        public DbSet<Segui> Segui { get; set; }
        public DbSet<Segnalazione> Segnalazioni { get; set; }
        public DbSet<Like> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commento>().HasOne(c => c.ApplicationUser).WithMany(p => p.Commenti).OnDelete(DeleteBehavior.ClientNoAction);
            base.OnModelCreating(modelBuilder);
            
        }
    }
}
