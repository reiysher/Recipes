using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Endpoints;
using StateSync.Api.Shared.Entities;
using StateSync.Api.Shared.Persistence;

namespace StateSync.Api.Features.Users;

internal static class GetUser
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("users/{userId:guid}", Get)
                .AllowAnonymous();
        }

        private static async Task<IResult> Get(
            [FromRoute] Guid userId,
            [FromServices] Handler handler,
            CancellationToken cancellationToken)
        {
            var request = new Request(userId);
            return await handler.Handle(request, cancellationToken);
        }
    }

    public sealed record Request(Guid UserId);

    internal sealed class Handler(ApplicationDbContext dbContext)
    {
        public async Task<IResult> Handle(Request query, CancellationToken cancellationToken)
        {
            var userProjection = await dbContext
                .Set<UserProjection>()
                .SingleOrDefaultAsync(p => p.Id == query.UserId, cancellationToken);

            if (userProjection is null)
            {
                return Results.NotFound("User not found");
            }

            return Results.Ok(userProjection);
        }
    }
}