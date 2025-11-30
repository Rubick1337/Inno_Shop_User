using Application.dto.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery(
        string? Name,
        string? Email,
        string? Role
    ) : IRequest<IEnumerable<ReadUserDto>>;
}
