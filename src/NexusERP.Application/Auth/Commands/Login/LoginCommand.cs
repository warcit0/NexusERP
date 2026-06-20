using MediatR;

namespace NexusERP.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
