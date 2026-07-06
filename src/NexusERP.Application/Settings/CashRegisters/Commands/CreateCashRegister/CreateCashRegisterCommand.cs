using MediatR;

namespace NexusERP.Application.Settings.CashRegisters.Commands.CreateCashRegister;

public record CreateCashRegisterCommand(Guid BranchId, string Name, string MacAddress) : IRequest<Guid>;
