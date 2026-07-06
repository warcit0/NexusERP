using MediatR;

namespace NexusERP.Application.Catalog.Brands.Commands.CreateBrand;

public record CreateBrandCommand(string Name, string Description) : IRequest<Guid>;
