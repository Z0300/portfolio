using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Features.Models;

public class Project
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public required string Name { get; set; }
    [Required, MaxLength(1000)]
    public required string Description { get; set; }
    public required string[] TechStack { get; set; }
    public List<CoreFeature> CoreFeatures { get; set; } = [];
    [MaxLength(255)]
    public string? RepositoryLink { get; set; }
}

