using Application.Products.Queries.GetAllProducts;
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
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly GetProductByIdQueryHandler _handler;

        public GetByIdProductHandlerTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);
        }
        private GetProductByIdQuery CreateQuery(int id)
        {
            return new GetProductByIdQuery(id);
        }
        [Fact]
        public async Task GetByIdProduct_Should_Get_Product()
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
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            var query = new GetProductByIdQuery(10);

            var result = await _handler.Handle(query, CancellationToken.None);

            _productRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()),Times.Once());
            Assert.Equal("Мышка", result.Name);
        }
    }
}
