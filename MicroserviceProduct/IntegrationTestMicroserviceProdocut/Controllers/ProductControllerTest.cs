using Application.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Inno_Shop_Product;

namespace IntegrationTestMicroserviceProdocut.Controllers
{
    public class ProductControllerTest
    {
        [Fact]
        public async Task GetAllProduct_Should_ReturnProducts()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/products");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<ReadProductDto>>();
            Assert.NotNull(products);
        }
        [Fact]
        public async Task GetById_NotExistingProduct_NotFound()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/products/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetById_Should_ReturnProduct()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/products/3");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var product = await response.Content.ReadFromJsonAsync<ReadProductDto>();
            Assert.NotNull(product);
        }
        [Fact]
        public async Task CreateProduct_WithoutAuth()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var newProduct = new CreateProductDto(
                "Мышка",
                "Мышка игровая",
                590m,
                true
            );
            var response = await client.PostAsJsonAsync("/api/products", newProduct);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task DeleteProduct_WithoutAuth()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("/api/products/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task SoftDeleteProduct_Should_Return()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.PatchAsync("/api/products/1/softdelete", content: null);

            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
