using MediatR;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Identity.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.TenantId);

        if (userId == null)
        {
            throw new InvalidOperationException($"No se pudo crear el usuario: {request.Email}. El email ya existe o la contraseña no cumple los requisitos.");
        }

        await _identityService.AddUserToRoleAsync(userId, request.Role);

        return new CreateUserResponse
        {
            UserId = userId,
            Email = request.Email,
            Role = request.Role,
            TenantId = request.TenantId
        };
    }
}
