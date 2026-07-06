using MediatR;

namespace NexusERP.Application.Catalog.Taxes.Queries.GetTaxes;

public record TaxDto(Guid Id, string Name, decimal Percentage, bool IsActive);

public record GetTaxesQuery : IRequest<List<TaxDto>>;
