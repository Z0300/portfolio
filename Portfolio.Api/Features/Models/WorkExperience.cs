using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Features.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Company { get; set; }

        [Required, MaxLength(100)]
        public required string Role { get; set; }

        [Required]
        public DateTime DateStarted { get; set; }

        public DateTime? DateEnded { get; set; }
        public List<Responsibility> Responsibilities { get; set; } = [];
    }
}
