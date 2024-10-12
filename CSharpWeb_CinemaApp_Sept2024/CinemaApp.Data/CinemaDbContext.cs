namespace CinemaApp.Data
{
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    public class CinemaDbContext: DbContext
    {

        public CinemaDbContext()
        {
        }

        public CinemaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Movie> Movies { get; set; } = null!;
        public virtual DbSet<Cinema> Cinemas { get; set; } = null!;
        public virtual DbSet<CinemaMovie> CinemasMovies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
