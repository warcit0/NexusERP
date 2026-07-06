using MediatR;

namespace NexusERP.Application.Catalog.Brands.Queries.GetBrands;

public record BrandDto(Guid Id, string Name, string Description, bool IsActive);

public record GetBrandsQuery : IRequest<List<BrandDto>>;
