using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CodeGenerator : ICodeGenerator
    {
        public string GenerateCode(int length)
        {
            int min = 10000;
            int max = 999999;

            return Random.Shared.Next(min, max).ToString();
        }
    }
}
