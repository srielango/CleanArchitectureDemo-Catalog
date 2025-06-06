using Catalog.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Products.Queries.GetPagedProducts;

public class GetPagedProductsHandler : IRequestHandler<GetPagedProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPagedProductsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.PageNumber - 1) * request.PageSize;

        return await _context.Products
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            })
            .ToListAsync(cancellationToken);
    }
}

