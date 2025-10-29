using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public static class GetProject
{
    public record Response(int Id, string Title, string Description, string[] TechStack, string? RepoUrl);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("projects/{id}", Handler).WithTags(Tags.PROJECTS);
        }
    }

    public static async Task<IResult> Handler(int id, AppDbContext context)
    {
        var product = await context.Projects.FindAsync(id);

        if (product is null)
        {
            return Results.NotFound(Result<Response>.Fail("Not found."));
        }
        var response = new Response(product.Id, product.Title, product.Description, product.TechStack, product.RepoUrl);

        return Results.Ok(Result<Response>.Ok(response, "Retrieved successfully."));
    }

}
