using FluentValidation;

namespace Catalog.Application.Products.Commands.CreateProduct;


public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.product.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.");

        RuleFor(x => x.product.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.product.Description)
            .MaximumLength(500).WithMessage("Description can’t exceed 500 characters.");
    }
}
