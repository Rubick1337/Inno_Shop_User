using Application.Products.Commands.SoftDeleteProduct;
using Application.Products.Commands.SoftUnDeleteProduct;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsMicroserviceProduct.Products.Commands
{
    public class SoftUnDeleteProductCommandHandlerTest
    {
        [Fact]
        public async Task SoftUnDeleteProduct_Should_SoftUnDelete()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var handler = new SoftUnDeleteProductCommandHandler(productRepositoryMock.Object);
            var command = new SoftUnDeleteProductCommand(UserId: 5);

            await handler.Handle(command, CancellationToken.None);

            productRepositoryMock.Verify(r => r.SoftUnDeleteAsync(5), Times.Once);
        }
    }
}
