using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Features.Models;

public class Responsibility
{
    public int Id { get; set; }

    [Required, MaxLength(10000)]
    public required string Content { get; set; }

    [ForeignKey(nameof(Experience))]
    public int ExperienceId { get; set; }
    public WorkExperience Experience { get; set; } = default!;
}
