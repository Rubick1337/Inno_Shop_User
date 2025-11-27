using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Product
{
    public record ReadProductDto(string Name, string Description, decimal Price, bool IsAvailable, bool IsDeleted , int UserId);
}
