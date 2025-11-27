using Application.Dto.Product;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Queries.GetByIdProduct
{
    public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ReadProductDto>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<ReadProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            return new ReadProductDto(
                product.Name,
                product.Description,
                product.Price,
                product.IsAvailable,
                product.IsDeleted,
                product.UserId,
                product.CreatedAt); 
        }
    }
}
