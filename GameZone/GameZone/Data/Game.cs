using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static GameZone.Common.ValidationConstants;

namespace GameZone.Data
{
    public class Game
    {
        [Key]
        [Comment("The primary key for the game.")]
        public int Id { get; set; }

        [Required]
        [MaxLength(GameTitleMaxLength)]
        [Comment("The title of the game.")]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(GameDescriptionMaxLength)]
        [Comment("The description of the game.")]
        public string Description { get; set; } = null!;

        [Comment("The URL of the image for the game.")]
        public string? ImageUrl { get; set; }

        [Required]
        [MaxLength(50)]
        [Comment("Identifier of the game Publisher")]
        public string PublisherId { get; set; } = null!;

        [ForeignKey(nameof(PublisherId))]
        public IdentityUser Publisher { get; set; } = null!;

        [Required]
        public DateTime ReleasedOn { get; set; }

        [Required]
        [Comment("Game genre")]
        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; } = null!;

        public IList<GamerGame> GamersGame { get; set; } = new List<GamerGame>();

        [Comment("Shows wether game is deleted or not")]
        public bool IsDeleted { get; set; }
    }
}
