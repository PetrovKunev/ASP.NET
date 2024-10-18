using SeminarHub.Data;
using System.ComponentModel.DataAnnotations;

namespace SeminarHub.Models
{
    public class SeminarCreateViewModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [MinLength(5)]
        [MaxLength(60)]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        [MaxLength(500)]
        public string Details { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateAndTime { get; set; }

        [Range(30, 180)]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();// За попълване на падащото меню с категории
    }
}
