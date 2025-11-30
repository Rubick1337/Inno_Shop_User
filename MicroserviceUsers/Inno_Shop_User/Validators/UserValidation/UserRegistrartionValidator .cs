using FluentValidation;
using Domain.Models;
using Application.dto.Auth;

namespace Inno_Shop_User.Validators.Validation
{
    public class UserRegistrartionValidator : AbstractValidator<RegisterUserDto>
    {
        public UserRegistrartionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно")
                .MinimumLength(2).WithMessage("Имя должно содержать минимум 2 символа");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен быть не менее 6 символов");
        }
    }
}
