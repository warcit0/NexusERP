using MediatR;

namespace NexusERP.Application.Settings.CashRegisters.Queries.GetCashRegisters;

public record CashRegisterDto(Guid Id, Guid BranchId, string BranchName, string Name, string MacAddress, bool IsActive, bool IsOpen);

public record GetCashRegistersQuery(Guid? BranchId = null) : IRequest<List<CashRegisterDto>>;
