using MediatR;

namespace NexusERP.Application.Settings.CashRegisters.Commands.CloseSession;

public class CloseSessionCommand : IRequest<bool>
{
    public Guid CashRegisterId { get; set; }
    public decimal FinalAmount { get; set; }
    public string Notes { get; set; } = string.Empty;
}
