using Application.Dto.Product;
using Application.Products.Commands.CreateProduct;
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
    public class CreateProductCommandHandlerTest
    {
        [Fact]
        public async Task CreateProduct_Should_Create()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            var dto = new CreateProductDto(
                Name: "Мышка",
                Description: "Игровая мышь",
                Price: 999m,
                IsAvailable: true
            );
            var command = new CreateProductCommand(productDto: dto, UserId: 10);
            var handler = new CreateProductCommandHandler(productRepositoryMock.Object);
            await handler.Handle(command, CancellationToken.None);
            productRepositoryMock.Verify(r => r.CreateAsync(
                It.Is<Product>(p =>
                    p.Name == dto.Name &&
                    p.Description == dto.Description &&
                    p.Price == dto.Price &&
                    p.IsAvailable == dto.IsAvailable &&
                    p.IsDeleted == false &&
                    p.UserId == command.UserId
                )),Times.Once);
        }
    }
}

