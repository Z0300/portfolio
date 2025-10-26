
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public static class GetProjects
{
    public record Response(int Id, string Title, string Description);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("projects", Handler).WithTags(Tags.PROJECTS);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context)
    {
        var products = await context.Projects.ToListAsync();

        var responses = products.Select(p => new Response(p.Id, p.Title, p.Description)).ToList();

        return Results.Ok(Result<List<Response>>.Ok(responses, "Retrieved successfully."));
    }

}
