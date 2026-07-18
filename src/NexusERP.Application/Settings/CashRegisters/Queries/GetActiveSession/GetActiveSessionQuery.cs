using MediatR;

namespace NexusERP.Application.Settings.CashRegisters.Queries.GetActiveSession;

public class GetActiveSessionQuery : IRequest<ActiveSessionDto?>
{
    public Guid CashRegisterId { get; set; }
}

public class ActiveSessionDto
{
    public Guid Id { get; set; }
    public Guid CashRegisterId { get; set; }
    public Guid BranchId { get; set; }
    public decimal InitialAmount { get; set; }
    public DateTime OpenedAt { get; set; }
    public string OpenedByUserId { get; set; } = string.Empty;
}
