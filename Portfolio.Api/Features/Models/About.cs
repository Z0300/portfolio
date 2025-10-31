using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Features.Models;

public class About
{
    public Guid Id { get; set; }
    [Required, MaxLength(2000)]
    public required string AboutMe { get; set; }

    [ForeignKey(nameof(Project))]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
