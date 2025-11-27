using Application.Dto.Product;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(int? UserId = null, 
        string? Name = null,
        decimal? MinPrice = null, 
        decimal? MaxPrice = null) : IRequest<IEnumerable<ReadProductDto>>;
}
