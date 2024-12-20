﻿using EventsSync.Api.Shared.Endpoints;
using EventsSync.Api.Shared.Entities;
using EventsSync.Api.Shared.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventsSync.Api.Features.Users;

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
            var user = await dbContext
                .Set<UserReadModel>()
                .SingleOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(user);
        }
    }
}