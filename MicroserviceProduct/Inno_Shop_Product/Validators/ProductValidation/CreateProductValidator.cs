using Application.Dto.Product;
using FluentValidation;

namespace Inno_Shop_Product.Validators.ProductValidation
{
    public class CreateProductValidator :AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator() {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательное поле")
                .MinimumLength(3).WithMessage("Имя должно содержать минимум 3 символа");
            RuleFor(x => x.Description)
                .MinimumLength(6).WithMessage("Описание должно содержать минимум 6 символов")
                .NotEmpty().WithMessage("Описание обязательное поле");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Цена обязательное поле")
                .Must(x => x >= 0).WithMessage("Цена не может быть отрицатильной");
        }
    }
}
