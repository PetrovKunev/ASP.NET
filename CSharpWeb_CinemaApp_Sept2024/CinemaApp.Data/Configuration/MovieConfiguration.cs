
namespace CinemaApp.Data.Configuration
{
    using Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static Common.EntityValidationConstants.Movie;
    internal class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .HasMaxLength(TitleMaxLength)
                .IsRequired();
        }
    }
}
