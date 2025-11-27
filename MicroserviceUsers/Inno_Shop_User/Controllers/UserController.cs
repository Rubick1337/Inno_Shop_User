using Application.dto.User;
using Application.Interfaces;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.SetUserActiveStatus;
using Application.Users.Commands;
using Application.Users.Queries.GetAllUsers;
using Application.Users.Queries.GetUserById;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop_User.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IMediator mediator,
        IValidator<UpdateUserDto> updateValidator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        IValidator<UpdateUserDto> _updateValidator = updateValidator;

       [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? email, [FromQuery] string? role)
        {
            var users = await _mediator.Send(new GetAllUsersQuery(name, email, role));
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            if (user is null)
                return NotFound("Пользователь не найден");

            return Ok(user);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            await _mediator.Send(new UpdateUserCommand(id, dto));
            return Ok("Updated");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return Ok("Deleted");
        }

        [HttpPatch("{id:int}/active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetActive(int id, bool active)
        {
            await _mediator.Send(new SetUserActiveStatusCommand(id, active));
            return Ok(new { message = "Статус обновлён" });
        }
    }
}
