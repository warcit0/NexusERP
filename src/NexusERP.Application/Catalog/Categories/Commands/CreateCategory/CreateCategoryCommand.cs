using MediatR;

namespace NexusERP.Application.Catalog.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Description) : IRequest<Guid>;
