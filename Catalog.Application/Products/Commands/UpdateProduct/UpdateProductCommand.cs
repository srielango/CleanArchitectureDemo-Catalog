using MediatR;

namespace Catalog.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(ProductDto product)
    : IRequest<bool>;
