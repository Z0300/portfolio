
using FluentValidation;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public static class UpdateProject
{
    public record Request(string Title, string Description, string[] TechStack, List<CoreFeatureRequest> CoreFeatures, string? RepositoryLink);
    public record CoreFeatureRequest(string Feature);
    public record Response(int Id, string Title);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.TechStack)
                .NotEmpty()
                .Must(stack => stack.All(s => !string.IsNullOrWhiteSpace(s)))
                .WithMessage("Each technology name must be non-empty.");

            RuleFor(x => x.RepositoryLink)
                .MaximumLength(255);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("projects/{id:int}", Handler)
                .WithTags(Tags.PROJECTS)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(int id, Request request, AppDbContext context)
    {
        var project = await context.Projects.FindAsync(id);

        if (project is null)
        {
            return Results.NotFound(Result<Response>.Fail("Not found."));
        }

        project.Name = request.Title;
        project.Description = request.Description;
        project.TechStack = request.TechStack;
        project.RepositoryLink = request.RepositoryLink;
        project.CoreFeatures = [.. request.CoreFeatures.Select(cf => new CoreFeature { Feature = cf.Feature })];

        await context.SaveChangesAsync();
        return Results.Ok(Result<Response>.Ok(new Response(project.Id, project.Name), "Updated successfully."));
    }
}
