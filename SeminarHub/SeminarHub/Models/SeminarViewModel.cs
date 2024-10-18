namespace SeminarHub.Models
{
    public class SeminarViewModel
    {
        public int Id { get; set; }

        public required string Topic { get; set; }

        public required string Lecturer { get; set; }

        public string Category { get; set; } = null!;

        public DateTime DateAndTime { get; set; }

        public required string Organizer { get; set; }

        public string Details { get; set; } = string.Empty;

        public int Duration { get; set; }
    }
}
