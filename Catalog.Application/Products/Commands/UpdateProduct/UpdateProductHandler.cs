using Catalog.Application.Common.Interfaces;
using MediatR;

namespace Catalog.Application.Products.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.product.Id }, cancellationToken);
        if (product is null) return false;

        product.Name = request.product.Name;
        product.Description = request.product.Description;
        product.Price = request.product.Price;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
