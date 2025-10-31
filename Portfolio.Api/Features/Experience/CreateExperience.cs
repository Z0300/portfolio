using FluentValidation;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Experience;

public static class CreateExperience
{
    public record Request(string Company, string Role, List<ResponsibilityRequest> Responsibilities, DateTime StartDate, DateTime? EndDate);
    public record ResponsibilityRequest(string Content);
    public record Response(int Id, string Company, string Role);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Company)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Role)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.Responsibilities)
                .NotEmpty()
                .WithMessage("At least one responsibility must be specified.");

            RuleFor(x => x.StartDate)
                            .NotEmpty()
                            .LessThanOrEqualTo(DateTime.UtcNow)
                            .WithMessage("Start date cannot be in the future.");

            RuleForEach(x => x.Responsibilities)
            .ChildRules(responsibility =>
            {
                responsibility.RuleFor(r => r.Content)
                    .NotEmpty()
                    .WithMessage("Responsibility title is required.")
                    .MaximumLength(1000)
                    .WithMessage("Responsibility title must not exceed 100 characters.");
            });
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("experiences", Handler)
                .WithTags(Tags.EXPERIENCES)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(Result<Response>.Fail("Validation failed"));
        }

        var experience = new WorkExperience
        {
            Company = request.Company,
            Role = request.Role,
            DateStarted = request.StartDate,
            DateEnded = request.EndDate,
            Responsibilities = [.. request.Responsibilities.Select(r => new Responsibility { Content = r.Content })],
        };

        context.Experiences.Add(experience);

        await context.SaveChangesAsync();

        var response = new Response(experience.Id, experience.Company, experience.Role);

        return Results.Ok(Result<Response>.Ok(response, "Successfully created."));
    }
}
