using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public static class RemoveProject
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("projects/{id}", Handler)
                .WithTags(Tags.PROJECTS)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(int id, AppDbContext context)
    {
        var product = await context.Projects.FindAsync(id);

        if (product is null)
        {
            return Results.NotFound(Result<string>.Fail("Not found."));
        }

        context.Remove(product);

        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}
