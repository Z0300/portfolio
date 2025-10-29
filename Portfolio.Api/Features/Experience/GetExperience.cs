using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Experience;

public static class GetExperience
{
    public record Response(int Id, string Company, string Role, DateTime StartDate, DateTime? EndDate, List<ResponsibilityResponse> Responsibilities);
    public record ResponsibilityResponse(int Id, string Content);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("experiences/{id:int}", Handler).WithTags(Tags.EXPERIENCES);
        }
    }

    public static async Task<IResult> Handler(int id, AppDbContext context)
    {
        var experience = await context.Experiences
            .Include(e => e.Responsibilities)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (experience is null)
            return Results.NotFound(Result<Response>.Fail("Not found"));

        var response = new Response(
            experience.Id,
            experience.Company,
            experience.Role,
            experience.StartDate,
            experience.EndDate,
            [.. experience.Responsibilities
                .OrderBy(r => r.Id)
                .Select(r => new ResponsibilityResponse(r.Id, r.Content))]
        );

        return Results.Ok(Result<Response>.Ok(response, "Successfully retrieved."));
    }
}
