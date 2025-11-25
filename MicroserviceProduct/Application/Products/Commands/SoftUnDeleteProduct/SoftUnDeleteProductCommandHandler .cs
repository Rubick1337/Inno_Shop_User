using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Commands.SoftUnDeleteProduct
{
    public class SoftUnDeleteProductCommandHandler(IProductRepository productRepository) : IRequestHandler<SoftUnDeleteProductCommand>
    {
        private readonly IProductRepository _productRepository = productRepository;
        public async Task Handle(SoftUnDeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.SoftUnDeleteAsync(request.UserId);
        }
    }
}
