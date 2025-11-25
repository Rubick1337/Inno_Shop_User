using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string Email, string Code) : IRequest;
}
