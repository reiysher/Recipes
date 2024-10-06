using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using StateAsync.Api.Shared.Abstractions;
using StateAsync.Api.Shared.Endpoints;

namespace StateAsync.Api.Features.Users;

internal static class ChangeUserPersonalInfo
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/change-personal-info", ChangePersonalInfo)
                .Accepts<Request>(MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        private static async Task<IResult> ChangePersonalInfo(
            [FromBody] Request request,
            [FromServices] Handler handler,
            CancellationToken cancellationToken)
        {
            return await handler.Handle(request, cancellationToken);
        }
    }

    public sealed record Request(Guid UserId, string FirstName, string LastName, string? MiddleName);

    internal sealed class Handler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        public async Task<IResult> Handle(Request command, CancellationToken cancellationToken)
        {
            var user = await userRepository.Get(command.UserId, cancellationToken);

            if (user == null)
            {
                return Results.NotFound("User not found");
            }

            user.ChangePersonalInfo(command.FirstName, command.LastName, command.MiddleName);

            await unitOfWork.Commit(cancellationToken);

            return Results.NoContent();
        }
    }
}