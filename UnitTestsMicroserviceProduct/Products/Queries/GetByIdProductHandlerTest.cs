using Application.Products.Queries.GetByIdProduct;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsMicroserviceProduct.Products.Queries
{
    public class GetByIdProductHandlerTest
    {
        [Fact]
        public async Task GetByIdProduct_Should_Get_Product()
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
            productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            var query = new GetProductByIdQuery(10);
            var handler = new GetProductByIdQueryHandler(productRepositoryMock.Object);

            var result = await handler.Handle(query, CancellationToken.None);

            productRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()),Times.Once());
            Assert.Equal("Мышка", result.Name);
        }
    }
}
