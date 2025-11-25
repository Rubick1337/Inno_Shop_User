using Application.dto.Auth;
using FluentValidation;

namespace Inno_Shop_User.Validators.UserValidation
{
    public class UserLoginValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginValidator() {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен");
        }
    }
}
