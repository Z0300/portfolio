
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Portfolio.Api.Features.Models;
using Portfolio.Api.Features.Projects;

namespace Portfolio.Api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkExperience> Experiences => Set<WorkExperience>();
    public DbSet<Responsibility> ExperienceDescriptions => Set<Responsibility>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<UniversalUtcValueConverter>();

        configurationBuilder
            .Properties<DateTime?>()
            .HaveConversion<NullableUniversalUtcValueConverter>();
    }
}

public class UniversalUtcValueConverter : ValueConverter<DateTime, DateTime>
{
    public UniversalUtcValueConverter()
        : base(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

public class NullableUniversalUtcValueConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUniversalUtcValueConverter()
        : base(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
    {
    }
}