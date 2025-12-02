using Application.Dto.Product;
using Application.Products.Commands.SoftUnDeleteProduct;
using Application.Products.Commands.UpdateProduct;
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
    public class UpdateProductCommandHandlerTest
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new UpdateProductCommandHandler(_productRepositoryMock.Object);
        }
        private UpdateProductCommand CreateCommand(int id,UpdateProductDto dto, int userId)
        {
            return new UpdateProductCommand(id,dto,userId);
        }

        [Fact]
        public async Task UpdateProduct_Product_NotFound()
        {
            var dto = new UpdateProductDto(
                Name: "Name",
                Description: "Descrption",
                Price: 100,
                IsAvailable: true
            );
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);
            var command = CreateCommand(1,dto,10);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Product>()), Times.Never);
        }
        [Fact]
        public async Task UpdateProduct_WrongUser()
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
            var dto = new UpdateProductDto(
                Name: "Name",
                Description: "Мышка игровая",
                Price: 590,
                IsAvailable: true
            );
            var command = CreateCommand(1, dto,10);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Product>()), Times.Never);
        }
        [Fact]
        public async Task UpdateProduct_Should_Update()
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
            var dto = new UpdateProductDto(
                Name: "Name",
                Description: "Мышка игровая",
                Price: 590,
                IsAvailable: true
            );
            var command = CreateCommand(1, dto, 1);

            await _handler.Handle(command, CancellationToken.None);

            _productRepositoryMock.Verify(r => r.UpdateAsync(1,It.Is<Product>(p =>p.Name == "Name" )),Times.Once);
        }
    }
}
