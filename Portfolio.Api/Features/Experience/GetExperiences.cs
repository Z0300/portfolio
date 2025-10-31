using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Experience;

public static class GetExperiences
{
    public record Response(
        int Id,
        string Company,
        string Role,
        DateTime StartDate,
        DateTime? EndDate,
        List<ResponsibilityResponse> Responsibilities);
    public record ResponsibilityResponse(int Id, string Content);
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
            .OrderByDescending(e => e.DateStarted)
            .AsNoTracking()
            .Select(e => new Response(
                    e.Id,
                    e.Company,
                    e.Role,
                    e.DateStarted,
                    e.DateEnded,
                    e.Responsibilities
                        .OrderBy(r => r.Id)
                        .Select(r => new ResponsibilityResponse(r.Id, r.Content))
                        .ToList()
            ))
            .ToListAsync();

        return Results.Ok(Result<List<Response>>.Ok(
            groupedExperiences,
            "Successfully retrieved."
        ));
    }

}
