using System.ComponentModel.DataAnnotations;

namespace SeminarHub.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Seminar> Seminars { get; set; } = new HashSet<Seminar>();
    }
}