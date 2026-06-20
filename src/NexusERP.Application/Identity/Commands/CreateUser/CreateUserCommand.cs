using MediatR;

namespace NexusERP.Application.Identity.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string Password,
    string Role,
    Guid? TenantId
) : IRequest<CreateUserResponse>;
