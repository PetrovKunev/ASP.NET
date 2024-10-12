using CinemaApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CinemaApp.Common.EntityValidationConstants.Cinema;

namespace CinemaApp.Data.Configuration
{
    public class CinemaConfiguration : IEntityTypeConfiguration<Cinema>
    {
        public void Configure(EntityTypeBuilder<Cinema> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            builder.Property(c => c.Location)
                .HasMaxLength(LocationMaxLength)
                .IsRequired();

            builder.HasData(this.GenerateCinemas());
        }

        private IEnumerable<Cinema> GenerateCinemas()
        {
            IEnumerable<Cinema> cinemas = new List<Cinema>
            {
                new Cinema
                {
                    Name = "Cinema City",
                    Location = "Sofia"
                },
                new Cinema
                {
                    Name = "Cine Grand",
                    Location = "Plovdiv"
                },
                new Cinema
                {
                    Name = "Cine Grand",
                    Location = "Varna"
                },


            };

            return cinemas;
        }
    }
}
