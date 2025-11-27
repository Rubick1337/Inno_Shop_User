using Domain.Interfaces;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<CreateProductCommand>
    {
        private readonly IProductRepository _productRepository = productRepository;
        public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var dto = request.productDto;

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                IsAvailable = dto.IsAvailable,
                IsDeleted = false,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.CreateAsync(product);
        }
    }
}
