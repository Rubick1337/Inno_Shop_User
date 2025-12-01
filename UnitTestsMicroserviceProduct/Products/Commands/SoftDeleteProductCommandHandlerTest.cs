using Application.Products.Commands.DeleteProduct;
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
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly SoftDeleteProductCommandHandler _handler;

        public SoftDeleteProductCommandHandlerTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new SoftDeleteProductCommandHandler(_productRepositoryMock.Object);
        }
        private SoftDeleteProductCommand CreateCommand(int userId)
        {
            return new SoftDeleteProductCommand(userId);
        }

        [Fact]
        public async Task SoftDeleteProduct_Should_SoftDelete()
        {
            var command = new SoftDeleteProductCommand(UserId: 5);

            await _handler.Handle(command, CancellationToken.None);

            _productRepositoryMock.Verify(r => r.SoftDeleteAsync(5),Times.Once);
        }
    }
}
