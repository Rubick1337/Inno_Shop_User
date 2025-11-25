using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Commands.SoftDeleteProduct
{
    public class SoftDeleteProductCommandHandler(IProductRepository productRepository) : IRequestHandler<SoftDeleteProductCommand>
    {
        private readonly IProductRepository _productRepository = productRepository ;
        public async Task Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.SoftDeleteAsync(request.UserId);
        }
    }
}
