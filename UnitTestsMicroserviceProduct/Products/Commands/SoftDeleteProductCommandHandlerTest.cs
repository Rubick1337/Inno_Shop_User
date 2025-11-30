using Application.Products.Commands.SoftDeleteProduct;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsMicroserviceProduct.Products.Commands
{
    public class SoftDeleteProductCommandHandlerTest
    {
        [Fact]
        public async Task SoftDeleteProduct_Should_SoftDelete()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var handler = new SoftDeleteProductCommandHandler(productRepositoryMock.Object);
            var command = new SoftDeleteProductCommand(UserId: 5);

            await handler.Handle(command, CancellationToken.None);

            productRepositoryMock.Verify(r => r.SoftDeleteAsync(5),Times.Once);
        }
    }
}
