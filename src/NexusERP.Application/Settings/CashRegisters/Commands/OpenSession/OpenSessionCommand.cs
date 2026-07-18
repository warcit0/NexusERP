using MediatR;

namespace NexusERP.Application.Settings.CashRegisters.Commands.OpenSession;

public class OpenSessionCommand : IRequest<Guid>
{
    public Guid CashRegisterId { get; set; }
    public decimal InitialAmount { get; set; }
    public string Notes { get; set; } = string.Empty;
}
