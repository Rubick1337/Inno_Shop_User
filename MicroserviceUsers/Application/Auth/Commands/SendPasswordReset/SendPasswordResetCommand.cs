using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.SendPasswordReset
{
    public record SendPasswordResetCommand(string Email) : IRequest;
}
