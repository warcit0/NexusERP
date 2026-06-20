using MediatR;
using NexusERP.Application.Auth.Commands.Login;

namespace NexusERP.Application.Auth.Commands.Refresh;

public record RefreshCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;
