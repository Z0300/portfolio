

using FluentValidation;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Projects;

public class CreateProject
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
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("projects", Handler)
                .WithTags(Tags.PROJECTS)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(Result<Response>.Fail("Validation failed."));
        }

        var project = new Project
        {
            Name = request.Title,
            Description = request.Description,
            TechStack = request.TechStack,
            CoreFeatures = [.. request.CoreFeatures.Select(cf => new CoreFeature { Feature = cf.Feature })],
            RepositoryLink = request.RepositoryLink
        };

        context.Projects.Add(project);

        await context.SaveChangesAsync();

        var response = new Response(project.Id, project.Name);

        return Results.Ok(Result<Response>.Ok(response, "Successfully created."));
    }

}
