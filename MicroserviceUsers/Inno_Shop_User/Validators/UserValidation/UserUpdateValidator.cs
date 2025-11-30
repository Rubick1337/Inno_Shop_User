using Application.dto.User;
using FluentValidation;

namespace Inno_Shop_User.Validators.UserValidation
{
    public class UserUpdateValidator : AbstractValidator<UpdateUserDto>
    {
        public UserUpdateValidator() {
            RuleFor(x => x.Name)
                 .MinimumLength(2).WithMessage("Имя должно содержать минимум 2 символа");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Некорректный формат email");
        }
    }
}
