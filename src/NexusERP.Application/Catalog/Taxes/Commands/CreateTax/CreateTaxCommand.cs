using MediatR;

namespace NexusERP.Application.Catalog.Taxes.Commands.CreateTax;

public record CreateTaxCommand(string Name, decimal Percentage) : IRequest<Guid>;
