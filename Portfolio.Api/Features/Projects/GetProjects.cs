using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public static class GetProjects
{
    public record Response(int Id, string Title, string Description, string[] TechStacks, List<CoreFeatureResponse> CoreFeatures, string? RepositoryLink);
    public record CoreFeatureResponse(string Feature);
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("projects", Handler).WithTags(Tags.PROJECTS);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context)
    {
        var projects = await context.Projects
        .Include(cf => cf.CoreFeatures)
        .AsNoTracking()
        .ToListAsync();

        var responses = projects.Select(project => new Response(project.Id,
            project.Name,
            project.Description,
            project.TechStack,
            [.. project.CoreFeatures.Select(cf => new CoreFeatureResponse(cf.Feature))],
            project.RepositoryLink)).ToList();

        return Results.Ok(Result<List<Response>>.Ok(responses, "Retrieved successfully."));
    }

}
