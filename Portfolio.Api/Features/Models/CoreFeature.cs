using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Features.Models;

public class CoreFeature
{
    public int Id { get; set; }
    [Required, MaxLength(500)]
    public required string Feature { get; set; }
}