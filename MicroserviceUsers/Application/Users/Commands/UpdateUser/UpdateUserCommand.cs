using Application.dto.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public record UpdateUserCommand(int Id, UpdateUserDto Dto) : IRequest;
}
