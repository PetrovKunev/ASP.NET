namespace CinemaApp.Data
{
    using Models;
    using Microsoft.EntityFrameworkCore;

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
    }
}
