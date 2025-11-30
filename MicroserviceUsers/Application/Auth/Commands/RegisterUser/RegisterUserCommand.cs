using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string Name,
        string Email,
        string Password
    ) : IRequest<string>;
}
