using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Мышка",
                    Description = "Мышка игровая",
                    Price = 590,
                    IsAvailable = true,
                    IsDeleted = false,
                    UserId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Клавиатура",
                    Description = "Механическая",
                    Price = 1290,
                    IsAvailable = true,
                    IsDeleted = false,
                    UserId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Наушники",
                    Description = "Беспроводные",
                    Price = 1990,
                    IsAvailable = true,
                    IsDeleted = false,
                    UserId = 2
                },
                new Product
                {
                    Id = 4,
                    Name = "Монитор",
                    Description = "Oled",
                    Price = 290900,
                    IsAvailable = true,
                    IsDeleted = false,
                    UserId = 2
                }
);
        }
    }
}
