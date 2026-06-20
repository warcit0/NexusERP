using MediatR;
using NexusERP.Application.Common.Exceptions;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (result, authResponse) = await _identityService.LoginAsync(request.Email, request.Password);

        if (!result || authResponse == null)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        return authResponse;
    }
}
