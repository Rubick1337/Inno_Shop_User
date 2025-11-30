using Application.Products.Queries.GetAllProducts;
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
    public class GetAllProductQueryHandlerTest
    {
        [Fact]
        public async Task GetAllProductQuery_Should_GetProducts()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Мышка",
                    Description = "Мышка игровая",
                    Price = 590,
                    IsAvailable = true,
                    IsDeleted = false,
                    UserId = 1,
                    CreatedAt = new DateTime(2025, 11, 11,0,0,0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 2,
                    Name = "Клавиатура",
                    Description = "Механическая",
                    Price = 1290,
                    IsAvailable = true,
                    IsDeleted = false,
                    UserId = 1,
                    CreatedAt = new DateTime(2025, 11, 11, 0, 0, 0, DateTimeKind.Utc)
                },
            };
            productRepositoryMock.Setup(r => r.GetAll(It.IsAny<string?>(), It.IsAny<decimal?>(),
                        It.IsAny<decimal?>(), It.IsAny<bool?>(), It.IsAny<int?>())).ReturnsAsync(products);
            var query = new GetAllProductsQuery(null, null, null, null);
            var handler = new GetAllProductQueryHandler(productRepositoryMock.Object);

            var result = await handler.Handle(query, CancellationToken.None);

            productRepositoryMock.Verify(r => r.GetAll(It.IsAny<string?>(), It.IsAny<decimal?>(),
                        It.IsAny<decimal?>(), It.IsAny<bool?>(), It.IsAny<int?>()), Times.Once);
            Assert.Equal(2, result.Count());
            Assert.Equal("Мышка", products[0].Name);
            Assert.Equal("Клавиатура", products[1].Name);
        }

        [Fact]
        public async Task GetAllProducts_Should_Pass_Filters()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(r => r.GetAll(
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<bool?>(),
                It.IsAny<int?>())).ReturnsAsync(new List<Product>());
            var handler = new GetAllProductQueryHandler(productRepositoryMock.Object);
            var query = new GetAllProductsQuery(5,"Мышка",100,500,true);

            await handler.Handle(query, CancellationToken.None);

            productRepositoryMock.Verify(r =>r.GetAll("Мышка", 100, 500, true, 5),Times.Once);
        }
    }
}
