﻿using System.Net.Mime;
using EventsAsync.Api.Shared.Abstractions;
using EventsAsync.Api.Shared.Endpoints;
using EventsAsync.Api.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventsAsync.Api.Features.Users;

internal static class RegisterUser
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/register", Register)
                .Accepts<Request>(MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        private static async Task<IResult> Register(
            [FromBody] Request request,
            [FromServices] Handler handler,
            CancellationToken cancellationToken)
        {
            return await handler.Handle(request, cancellationToken);
        }
    }

    public sealed record Request(string FirstName, string LastName, string? MiddleName, string Email);

    internal sealed class Handler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        public async Task<IResult> Handle(Request command, CancellationToken cancellationToken)
        {
            var user = new User(
                Guid.NewGuid(),
                command.FirstName,
                command.LastName,
                command.MiddleName,
                command.Email);

            await userRepository.Save(user, cancellationToken);
            await unitOfWork.Commit(cancellationToken);

            return Results.Ok(user.Id);
        }
    }
}