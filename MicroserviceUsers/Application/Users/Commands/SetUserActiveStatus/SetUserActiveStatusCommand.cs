using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Users.Commands.SetUserActiveStatus
{
    public record SetUserActiveStatusCommand(int Id, bool IsActive) : IRequest;
}
