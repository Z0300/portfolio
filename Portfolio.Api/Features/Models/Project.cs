using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Features.Projects;

public class Project
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public required string Title { get; set; }
    [Required, MaxLength(1000)]
    public required string Description { get; set; }
    public required string[] TechStack { get; set; }
    [MaxLength(255)]
    public string? RepoUrl { get; set; }
}