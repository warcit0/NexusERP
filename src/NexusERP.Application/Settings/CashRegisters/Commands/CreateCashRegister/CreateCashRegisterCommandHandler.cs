using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Settings.CashRegisters.Commands.CreateCashRegister;

public class CreateCashRegisterCommandHandler : IRequestHandler<CreateCashRegisterCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateCashRegisterCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCashRegisterCommand request, CancellationToken cancellationToken)
    {
        var cashRegister = new CashRegister
        {
            BranchId = request.BranchId,
            Name = request.Name,
            MacAddress = request.MacAddress,
            IsActive = true,
            IsOpen = false
        };

        _context.CashRegisters.Add(cashRegister);
        await _context.SaveChangesAsync(cancellationToken);

        return cashRegister.Id;
    }
}
