using Application.Configuration;
using Application.Services;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Inno_Shop_User;
using System.Net.Http.Json;
using Application.dto.User;
namespace IntegrationTestMicroServiceUser.Controllers
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetAll_Should_GetUsers()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/users");

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            Assert.NotNull(users);
        }
        [Fact]
        public async Task GetById_Should_GetUser()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/users/3");

            response.EnsureSuccessStatusCode();


            var user = await response.Content.ReadFromJsonAsync<User>();

            Assert.NotNull(user);
        }
        [Fact]
        public async Task GetById_NotFound()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.GetAsync("/api/users/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Update_Should_Update()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var updateDto = new UpdateUserDto
            ("Updated","updated@gmail.com","Admin");

            var response = await client.PatchAsJsonAsync("/api/users/3", updateDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Delete_Should_Delete()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("/api/users/2");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Deleted", result);
        }
    }
}
