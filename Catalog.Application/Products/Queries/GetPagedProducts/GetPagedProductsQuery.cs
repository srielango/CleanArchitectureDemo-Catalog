using MediatR;

namespace Catalog.Application.Products.Queries.GetPagedProducts;

public record GetPagedProductsQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<List<ProductDto>>;
