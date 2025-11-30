using Application.Dto.Product;
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
        [Fact]
        public async Task UpdateProduct_Product_NotFound()
        {

            var dto = new UpdateProductDto(
                Name: "Name",
                Description: "Descrption",
                Price: 100,
                IsAvailable: true
            );
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);
            var command = new UpdateProductCommand(1,dto,10);
            var handler = new UpdateProductCommandHandler(productRepositoryMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));

            productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Product>()), Times.Never);
        }
        [Fact]
        public async Task UpdateProduct_WrongUser()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
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
            productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            var dto = new UpdateProductDto(
                Name: "Name",
                Description: "Мышка игровая",
                Price: 590,
                IsAvailable: true
            );
            var command = new UpdateProductCommand(1, dto,10);
            var handler = new UpdateProductCommandHandler(productRepositoryMock.Object);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(command, CancellationToken.None));

            productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Product>()), Times.Never);
        }
        [Fact]
        public async Task UpdateProduct_Should_Update()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
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
            productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            var dto = new UpdateProductDto(
                Name: "Name",
                Description: "Мышка игровая",
                Price: 590,
                IsAvailable: true
            );
            var command = new UpdateProductCommand(1, dto, 1);
            var handler = new UpdateProductCommandHandler(productRepositoryMock.Object);

            await handler.Handle(command, CancellationToken.None);

            productRepositoryMock.Verify(r => r.UpdateAsync(1,It.Is<Product>(p =>p.Name == "Name" )),Times.Once);
        }
    }
}
