﻿using System.Net.Mime;
using EventsSync.Api.Shared.Abstractions;
using EventsSync.Api.Shared.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace EventsSync.Api.Features.Users;

internal static class ChangeUserEmail
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/change-email", ChangeEmail)
                .Accepts<Request>(MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        private static async Task<IResult> ChangeEmail(
            [FromBody] Request request,
            [FromServices] Handler handler,
            CancellationToken cancellationToken)
        {
            return await handler.Handle(request, cancellationToken);
        }
    }

    public sealed record Request(Guid UserId, string Email);

    internal sealed class Handler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        public async Task<IResult> Handle(Request command, CancellationToken cancellationToken)
        {
            var user = await userRepository.Get(command.UserId, cancellationToken);

            if (user == null)
            {
                return Results.NotFound("User not found");
            }

            user.ChangeEmail(command.Email);

            await userRepository.Save(user, cancellationToken);
            await unitOfWork.Commit(cancellationToken);

            return Results.NoContent();
        }
    }
}