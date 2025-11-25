using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICodeGenerator
    {
        const int LENGTH = 6;
        string GenerateCode(int length = LENGTH);
    }
}
