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
        [Fact]
        public async Task DeleteProduct_Should_Delete()
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

            var command = new DeleteProductCommand(1, 1);
            var handler = new DeleteProductCommandHandler(productRepositoryMock.Object);

            await handler.Handle(command, CancellationToken.None);

            productRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once());
        }
    }
}
