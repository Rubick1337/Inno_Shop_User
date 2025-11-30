using Application.dto.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTestMicroServiceUser.Controllers
{
    public class AuthControllerTest
    {
        [Fact]
        public async Task RequestEmailConfirmation_Should_RequestEmail()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.PostAsync("/api/auths/request-email-confirmation?email=testuser@gmail.com", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("На почту отправлен код подтверждения", result);
        }
        [Fact]
        public async Task ConfirmEmail_Should_WrongCode()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            string code = "123456"; 
            string email = "testuser@example.com";

            var response = await client.PostAsync($"/api/auths/confirm-email?code={code}&email={email}", null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task RequestPasswordReset_Should_SendEmail()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var response = await client.PostAsync("/api/auths/request-password-reset?email=testuser@gmail.com", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Код восстановления отправлен", result);
        }
        [Fact]
        public async Task ResetPassword_Should_WrongCode()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            string email = "testuser@gmail.com";
            string code = "123456";  
            string newPassword = "1234567";

            var response = await client.PostAsync($"/api/auths/reset-password?email={email}&code={code}&newPassword={newPassword}", null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task Register_Should_ReturnOk_When_ValidData()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            var registerDto = new RegisterUserDto(
                "poss13372@gmail.com", "poss13372@gmail.com", "123456");

            var response = await client.PostAsJsonAsync("/api/auths/register", registerDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

}
