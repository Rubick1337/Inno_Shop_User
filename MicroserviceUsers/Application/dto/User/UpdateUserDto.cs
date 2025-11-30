using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.dto.User
{
    public record UpdateUserDto(
        string? Name,
        string? Email,
        string? Role
    );
}
