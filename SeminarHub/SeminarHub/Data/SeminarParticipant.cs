using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SeminarHub.Data
{
    public class SeminarParticipant
    {
        [Required]
        public int SeminarId { get; set; }

        [ForeignKey(nameof(SeminarId))]
        public virtual Seminar Seminar { get; set; }

        [Required]
        public string ParticipantId { get; set; }

        [ForeignKey(nameof(ParticipantId))]
        public virtual IdentityUser Participant { get; set; }
    }
}

