using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.SoftDeleteProduct;
using Application.Products.Commands.SoftUnDeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetAllProducts;
using Application.Products.Queries.GetByIdProduct;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inno_Shop_Product.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? userId, [FromQuery] string? name = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
        {
            var query = new GetAllProductsQuery(userId, name, minPrice, maxPrice);
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
        public async Task<IActionResult> Create([FromBody] Product product, [FromQuery] int userId)
        {
            var command = new CreateProductCommand(userId, product);
            await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct, [FromQuery] int userId)
        {
            var command = new UpdateProductCommand(id, updatedProduct, userId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{userId}/softdelete")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(int userId)
        {
            var command = new SoftDeleteProductCommand(userId);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{userId}/softundelete")]
        [Authorize]
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
            var command = new DeleteProductCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
