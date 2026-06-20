using MediatR;

namespace NexusERP.Application.Auth.Commands.Logout;

public record LogoutCommand(string AccessToken) : IRequest;
