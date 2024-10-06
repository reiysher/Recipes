using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StateSync.Api.Shared.Abstractions;
using StateSync.Api.Shared.Endpoints;

namespace StateSync.Api.Features.Users;

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

            await unitOfWork.Commit(cancellationToken);

            return Results.NoContent();
        }
    }
}