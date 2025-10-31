using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public static class GetProject
{
    public record Response(int Id, string Title, string Description, string[] TechStack, List<CoreFeatureResponse> CoreFeatures, string? RepoUrl);
    public record CoreFeatureResponse(string Feature);
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("projects/{id}", Handler).WithTags(Tags.PROJECTS);
        }
    }

    public static async Task<IResult> Handler(int id, AppDbContext context)
    {
        var product = await context.Projects
        .Include(cf => cf.CoreFeatures)
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return Results.NotFound(Result<Response>.Fail("Not found."));
        }
        var response = new Response(
            product.Id,
            product.Name,
            product.Description,
            product.TechStack,
            [.. product.CoreFeatures.Select(cf => new CoreFeatureResponse(cf.Feature))],
            product.RepositoryLink);

        return Results.Ok(Result<Response>.Ok(response, "Retrieved successfully."));
    }

}
