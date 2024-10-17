using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SeminarHub.Data
{
    public class Seminar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Topic { get; set; } = null!;

        [Required]
        [MinLength(5)]
        [MaxLength(60)]
        public string Lecturer { get; set; } = null!;

        [Required]
        [MinLength(10)]
        [MaxLength(500)]
        public string Details { get; set; } = null!;

        [Required]
        public string OrganizerId { get; set; } = null!;

        [ForeignKey(nameof(OrganizerId))]
        public virtual IdentityUser Organizer { get; set; } = null!;

        [Required]
        public DateTime DateAndTime { get; set; }

        [Range(30, 180)]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<SeminarParticipant> SeminarParticipants { get; set; } = new HashSet<SeminarParticipant>();

        public bool IsDeleted { get; set; }
    }
}
