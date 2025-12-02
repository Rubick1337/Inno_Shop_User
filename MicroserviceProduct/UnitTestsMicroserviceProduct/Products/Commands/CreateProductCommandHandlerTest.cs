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
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _handler = new CreateProductCommandHandler(_productRepositoryMock.Object);
        }
        private CreateProductCommand CreateCommand(CreateProductDto dto, int id)
        {
            return new CreateProductCommand(id, dto);
        }
        [Fact]
        public async Task CreateProduct_Should_Create()
        {
            var dto = new CreateProductDto(
                Name: "Мышка",
                Description: "Игровая мышь",
                Price: 999,
                IsAvailable: true
            );
            var command = CreateCommand(dto, 10);

            await _handler.Handle(command, CancellationToken.None);
            _productRepositoryMock.Verify(r => r.CreateAsync(
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

