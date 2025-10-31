using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;

namespace Portfolio.Api.Features.Experience;

public class UpdateExperience
{
    public record Request(string Company, string Role, List<ResponsibilityRequest> Responsibilities, DateTime StartDate, DateTime? EndDate);
    public record ResponsibilityRequest(int Id, string Content);
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
            app.MapPut("experiences/{id:int}", Handler)
                .WithTags(Tags.EXPERIENCES)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(int id, Request request, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(Result<Response>.Fail("Validation failed."));
        }

        var experience = await context.Experiences
            .Include(e => e.Responsibilities)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (experience is null)
        {
            return Results.NotFound(Result<Response>.Fail($"Experience with ID {id} not found."));
        }

        experience.Company = request.Company;
        experience.Role = request.Role;
        experience.DateStarted = request.StartDate;


        var existing = experience.Responsibilities;
        var updated = request.Responsibilities ?? [];


        var toRemove = existing
            .Where(r => !updated.Any(ur => ur.Id == r.Id))
            .ToList();

        if (toRemove.Count > 0)
            context.Resposibilities.RemoveRange(toRemove);


        foreach (var existingResp in existing)
        {
            var match = updated.FirstOrDefault(ur => ur.Id == existingResp.Id);
            if (match != null && existingResp.Content != match.Content)
            {
                existingResp.Content = match.Content;
            }
        }

        var toAdd = updated
            .Where(ur => ur.Id == 0) // Id == 0 means new item (not yet saved)
            .Select(ur => new Responsibility
            {
                Content = ur.Content,
                ExperienceId = experience.Id
            })
            .ToList();

        if (toAdd.Count > 0)
            experience.Responsibilities.AddRange(toAdd);


        await context.SaveChangesAsync();

        return Results.Ok(Result<Response>.Ok(
            new Response(experience.Id, experience.Company, experience.Role),
         "Successfully updated."));
    }

}
