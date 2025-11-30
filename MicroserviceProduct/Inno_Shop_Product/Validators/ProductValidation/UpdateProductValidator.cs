using Application.Dto.Product;
using FluentValidation;

namespace Inno_Shop_Product.Validators.ProductValidation
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator() {
            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage("Имя должно содержать минимум 3 символа");
            RuleFor(x => x.Description)
                .MinimumLength(6).WithMessage("Описание должно содержать минимум 6 символов");
            RuleFor(x => x.Price)
                .Must(x => x >= 0).WithMessage("Цена не может быть отрицатильной");
        }
    }
}
