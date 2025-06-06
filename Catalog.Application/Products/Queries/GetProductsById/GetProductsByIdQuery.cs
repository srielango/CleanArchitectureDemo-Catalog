using MediatR;

namespace Catalog.Application.Products.Queries.GetProductsById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
