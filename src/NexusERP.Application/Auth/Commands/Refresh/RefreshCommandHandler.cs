using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Application.Auth.Commands.Login;

namespace NexusERP.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public RefreshCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var response = await _identityService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

        if (response == null)
        {
            throw new UnauthorizedAccessException("Invalid token or refresh token.");
        }

        return response;
    }
}
