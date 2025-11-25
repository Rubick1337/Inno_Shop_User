using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.dto.User
{
    public record ReadUserDto(
        int Id,
        string Name,
        string Email,
        string Role,
        bool IsActived
    );
}
