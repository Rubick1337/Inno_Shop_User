    using Domain.Models;
    using MediatR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Application.Products.Queries.GetByIdProduct
    {
        public record GetProductByIdQuery(int Id) : IRequest<Product>;
    }
