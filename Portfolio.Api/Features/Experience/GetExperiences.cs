using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Experience;

public static class GetExperiences
{
    public record ResponsibilityResponse(int Id, string Content);
    public record ExperienceResponse(
        int Id,
        string Role,
        DateTime StartDate,
        DateTime? EndDate,
        List<ResponsibilityResponse> Responsibilities);
    public record Response(
        string Company,
        List<ExperienceResponse> Experiences);
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("experiences", Handler).WithTags(Tags.EXPERIENCES);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context)
    {
        var groupedExperiences = await context.Experiences
            .Include(e => e.Responsibilities)
            .OrderByDescending(e => e.StartDate)
            .AsNoTracking()
            .GroupBy(e => e.Company)
            .Select(g => new Response(
                g.Key,
                g.Select(e => new ExperienceResponse(
                    e.Id,
                    e.Role,
                    e.StartDate,
                    e.EndDate,
                    e.Responsibilities
                        .OrderBy(r => r.Id)
                        .Select(r => new ResponsibilityResponse(r.Id, r.Content))
                        .ToList()
                )).ToList()
            ))
            .ToListAsync();

        return Results.Ok(Result<List<Response>>.Ok(
            groupedExperiences,
            "Successfully retrieved."
        ));
    }

}
