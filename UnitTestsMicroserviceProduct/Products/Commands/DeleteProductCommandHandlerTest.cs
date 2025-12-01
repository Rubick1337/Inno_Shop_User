using Application.Dto.Product;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsMicroserviceProduct.Products.Commands
{
    public class DeleteProductCommandHandlerTest
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new DeleteProductCommandHandler(_productRepositoryMock.Object);
        }
        private DeleteProductCommand CreateCommand(int userId, int id)
        {
            return new DeleteProductCommand(userId, id);
        }
        [Fact]
        public async Task DeleteProduct_Should_Delete()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Мышка",
                Description = "Мышка игровая",
                Price = 590,
                IsAvailable = true,
                IsDeleted = false,
                UserId = 1,
                CreatedAt = new DateTime(2025, 11, 11, 0, 0, 0, DateTimeKind.Utc)
            };
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var command = CreateCommand(1, 1);

            await _handler.Handle(command, CancellationToken.None);

            _productRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once());
        }
    }
}
