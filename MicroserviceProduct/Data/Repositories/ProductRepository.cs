using Data.Context;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductRepository(ProductContext context) : IProductRepository
    {
        private readonly ProductContext _context = context;

        public async Task CreateAsync(Product product)
        {
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.products.
                AsNoTracking().
                FirstOrDefaultAsync(x => x.Id == id);

            if(product is null)
            {
                return;
            }

            _context.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product?>> GetAll(string? name = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            bool? isAvailable = null, 
            int? userId = null)
        {

            IQueryable<Product> query = _context.products.AsNoTracking();

            if(name is not null)
            {
                query = query.Where(x => x.Name == name);
            }

            if(isAvailable is not null)
            {
                query = query.Where(x => x.IsAvailable == isAvailable);
            }

            if (userId is not null)
            {
                query = query.Where(x => x.UserId == userId);
            }

            if (minPrice is not null)
            {
                query = query.Where(x => x.Price >= minPrice);
            }

            if (maxPrice is not null)
            {
                query = query.Where(x => x.Price <= maxPrice);
            }

            return await query.ToListAsync();

        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.products.
                AsNoTracking().
                FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SoftDeleteAsync(int userId)
        {
            var products = _context.products.Where(x => x.UserId == userId).ToList();

            foreach (var product in products)
            {
                product.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

        }

        public async Task SoftUnDeleteAsync(int userId)
        {
            var products = _context.products.Where(x => x.UserId == userId).ToList();

            foreach (var product in products)
            {
                product.IsDeleted = false;
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id , Product updatedProduct)
        {
            var productExisting = await _context.products.FirstOrDefaultAsync(x => x.Id == id);

            if (productExisting is null) {
                return;
            }

            productExisting.Id = id;

            _context.Entry(productExisting).CurrentValues.SetValues(updatedProduct);

            await _context.SaveChangesAsync();
        }
    }
}
