using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Features.Models;
using Sprache;
using Supabase;

namespace Portfolio.Api.Features.auth;

public static class UserLogin
{
    public record Request(string Email, string Password);
    public record Response(
      string AccessToken,
      string RefreshToken,
      long ExpiresIn,
      UserResponse User);

    public record UserResponse(
        string Id,
        string Email,
        string? DisplayName);


    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("auth", Handler)
                .WithTags(Tags.AUTH);
        }
    }

    public static async Task<IResult> Handler(Request request, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(Result<Response>.Fail("Validation failed"));
        }

        var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")!;
        var supabaseAnonKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY")!;

        var client = new Client(supabaseUrl, supabaseAnonKey, new SupabaseOptions
        {
            AutoConnectRealtime = false
        });

        await client.InitializeAsync();

        try
        {
            var session = await client.Auth.SignIn(request.Email, request.Password);
            if (session == null || session.User == null)
                return Results.BadRequest(Result<Response>.Fail("Invalid email or password."));

            var response = new Response(
                session.AccessToken!,
                session.RefreshToken!,
                session.ExpiresIn,
                 new UserResponse(
                    session.User.Id!,
                    session.User.Email!,
                    session.User.UserMetadata?.GetValueOrDefault("display_name")?.ToString()
            ));

            return Results.Ok(Result<Response>.Ok(response, "Login successful."));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(Result<Response>.Fail($"Login failed: {ex.Message}"));
        }
    }
}
