using Application.Dto.Product;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Queries.GetAllProducts
{
    public class GetAllProductQueryHandler(IProductRepository productRepository) : IRequestHandler<GetAllProductsQuery, IEnumerable<ReadProductDto>>
    {
        private readonly IProductRepository _productRepository = productRepository;
        public async Task<IEnumerable<ReadProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAll(
                name: request.Name,
                minPrice: request.MinPrice,
                maxPrice: request.MaxPrice,
                userId: request.UserId,
                isAvailable: request.IsAviable);

            return products.Select(p => new ReadProductDto(
                p.Name,
                p.Description,
                p.Price,
                p.IsAvailable,
                p.IsDeleted,
                p.UserId,
                p.CreatedAt
                )); ;

        }
    }
}
