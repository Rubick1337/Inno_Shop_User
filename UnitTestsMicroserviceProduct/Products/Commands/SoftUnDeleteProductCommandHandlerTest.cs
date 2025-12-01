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
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly SoftUnDeleteProductCommandHandler _handler;

        public SoftUnDeleteProductCommandHandlerTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new SoftUnDeleteProductCommandHandler(_productRepositoryMock.Object);
        }
        private SoftUnDeleteProductCommand CreateCommand(int userId)
        {
            return new SoftUnDeleteProductCommand(userId);
        }

        [Fact]
        public async Task SoftUnDeleteProduct_Should_SoftUnDelete()
        {
            var command = CreateCommand(5);

            await _handler.Handle(command, CancellationToken.None);

            _productRepositoryMock.Verify(r => r.SoftUnDeleteAsync(5), Times.Once);
        }
    }
}
