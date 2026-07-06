using MediatR;

namespace NexusERP.Application.Catalog.Categories.Queries.GetCategories;

public record CategoryDto(Guid Id, string Name, string Description, bool IsActive);

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
