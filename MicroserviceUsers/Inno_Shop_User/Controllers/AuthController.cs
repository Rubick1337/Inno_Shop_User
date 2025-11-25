using Application.Auth.Commands.ConfirmEmail;
using Application.Auth.Commands.LoginUser;
using Application.Auth.Commands.RegisterUser;
using Application.Auth.Commands.ResetPassword;
using Application.Auth.Commands.SendEmailConfirmation;
using Application.Auth.Commands.SendPasswordReset;
using Application.dto.Auth;
using Application.dto.User;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop_User.Controllers
{
    [ApiController]
    [Route("api/auths")]
    public class AuthController(IMediator mediator,
        IValidator<RegisterUserDto> registerValidator,
        IValidator<UserLoginDto> loginValidator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IValidator<RegisterUserDto> _registerValidator = registerValidator;
        private readonly IValidator<UserLoginDto?> _loginValidator = loginValidator;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var validation = await _registerValidator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState, null);
                return BadRequest(ModelState);
            }

            var token = await _mediator.Send(new RegisterUserCommand(dto.Name, dto.Email, dto.Password));
            return Ok(new { message = "Регистрация успешна, подтвердите аккаунт", token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var validation = await _loginValidator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState, null);
                return BadRequest(ModelState);
            }

            var token = await _mediator.Send(new LoginUserCommand(dto.Email, dto.Password));
            return Ok(new { token });
        }

        [HttpPost("request-email-confirmation")]
        public async Task<IActionResult> RequestEmailConfirmation(string email)
        {
            await _mediator.Send(new SendEmailConfirmationCommand(email));
            return Ok("На почту отправлен код подтверждения");
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string code,string email)
        {
            await _mediator.Send(new ConfirmEmailCommand(email,code));
            return Ok("Аккаунт подтвержден");
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            await _mediator.Send(new SendPasswordResetCommand(email));
            return Ok("Код восстановления отправлен");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string code, string newPassword)
        {
            await _mediator.Send(new ResetPasswordCommand(email, code, newPassword));
            return Ok("Пароль успешно обновлён");
        }
    }
}
