using MediatR;

namespace Catalog.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(ProductDto product)
    : IRequest<ProductDto>;
