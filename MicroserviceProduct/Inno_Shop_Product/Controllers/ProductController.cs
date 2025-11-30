using Application.Dto.Product;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.SoftDeleteProduct;
using Application.Products.Commands.SoftUnDeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetAllProducts;
using Application.Products.Queries.GetByIdProduct;
using Domain.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inno_Shop_Product.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController(IMediator mediator,
        IValidator<CreateProductDto> validatorCreateProduct,
        IValidator<UpdateProductDto> validatorUpdateProduct) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IValidator<CreateProductDto> _validatorCreateProduct = validatorCreateProduct;
        private readonly IValidator<UpdateProductDto> _validatorUpdateProduct = validatorUpdateProduct;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? userId, 
            [FromQuery] string? name = null, 
            [FromQuery] decimal? minPrice = null, 
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? IsAvailable = null)
        {
            var query = new GetAllProductsQuery(userId, name, minPrice, maxPrice, IsAvailable);
            var products = await _mediator.Send(query);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var product = await _mediator.Send(query);
            if (product is null)
            {
                return NotFound("Продукт не найден");
            }
            return Ok(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto productDto)
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Некорректный идентификатор пользователя в токене");
            var validation = _validatorCreateProduct.Validate(productDto);
            if(!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return BadRequest();
            }

            var command = new CreateProductCommand(userId, productDto);
            await _mediator.Send(command);
            return Ok(productDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updatedProductDto)
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Некорректный идентификатор пользователя в токене");

            var validation = _validatorUpdateProduct.Validate(updatedProductDto);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return BadRequest();
            }

            var command = new UpdateProductCommand(id, updatedProductDto, userId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{userId}/softdelete")]
        public async Task<IActionResult> SoftDelete(int userId)
        {
            var command = new SoftDeleteProductCommand(userId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{userId}/softundelete")]
        public async Task<IActionResult> SoftUnDelete(int userId)
        {
            var command = new SoftUnDeleteProductCommand(userId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Некорректный идентификатор пользователя в токене");

            var command = new DeleteProductCommand(id,userId);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
