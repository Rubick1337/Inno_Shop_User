using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository = productRepository;
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);

            if (existingProduct is null)
            {
                throw new InvalidOperationException("Продукт не найден");
            }

            if (existingProduct.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Вы не можете редактировать чужой продукт");
            }
            var dto = request.productDto;

            if (dto.Name is not null)
                existingProduct.Name = dto.Name;

            if (dto.Description is not null)
                existingProduct.Description = dto.Description;

            if (dto.Price.HasValue)
                existingProduct.Price = dto.Price.Value;

            if (dto.IsAvailable.HasValue)
                existingProduct.IsAvailable = dto.IsAvailable.Value;

            await _productRepository.UpdateAsync(request.Id, existingProduct);
        }
    }
}
