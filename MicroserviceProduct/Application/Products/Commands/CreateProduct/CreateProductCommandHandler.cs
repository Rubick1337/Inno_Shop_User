using Domain.Interfaces;
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
            var product = request.Product;
            product.UserId = request.UserId;
            await _productRepository.CreateAsync(product);
        }
    }
}
