using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Application.Catalog.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateCategoryCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
