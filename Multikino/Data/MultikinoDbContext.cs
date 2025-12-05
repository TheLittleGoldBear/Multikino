using Microsoft.EntityFrameworkCore;
using Multikino.Models;

namespace Multikino.Data
{
    public class MultikinoDbContext : DbContext
    {
        public MultikinoDbContext(DbContextOptions<MultikinoDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Screening> Screenings => Set<Screening>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<MovieImage> MovieImages => Set<MovieImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Screenings)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Screenings)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Images)
                .WithOne(i => i.Movie)
                .HasForeignKey(i => i.MovieId);

            modelBuilder.Entity<Screening>()
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Screening)
                .HasForeignKey(t => t.ScreeningId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tickets)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
