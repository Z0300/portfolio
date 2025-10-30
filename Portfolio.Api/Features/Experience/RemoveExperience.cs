using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Experience;

public class RemoveExperience
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("experiences/{id}", Handler)
                .WithTags(Tags.EXPERIENCES)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(int id, AppDbContext context)
    {
        var experience = await context.Experiences.FindAsync(id);

        if (experience is null)
        {
            return Results.NotFound(Result<string>.Fail("Not found."));
        }

        context.Remove(experience);

        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}
