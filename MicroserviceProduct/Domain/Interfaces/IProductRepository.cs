using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll(string? name = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isAvailable = null,
            int? userId = null);
        Task<Product> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id , Product product);
        Task SoftDeleteAsync(int userId);  
        Task SoftUnDeleteAsync(int userId);
        Task CreateAsync(Product product);
    }
}
