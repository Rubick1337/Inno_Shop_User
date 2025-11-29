using Application.Users.Queries.GetUserById;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMicroserviceUsers.Users.Queries
{
    public class GetUserByIdQueryHandlerTest
    {
        [Fact]
        public async Task GetUsersById_Should_Get_User()
        {
            var repositoryUserMock = new Mock<IUserRepository>();
            var user = new User
            {
                Id = 10,
                Name = "Test",
                Email = "test@gmail.com",
                PasswordHash = "hash",
                Role = "User",
                IsActived = false
            };
            repositoryUserMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);
            var query = new GetUserByIdQuery(Id: 10);
            var handler = new GetUserByIdQueryHandler(repositoryUserMock.Object);

            var result = await handler.Handle(query,CancellationToken.None);

            repositoryUserMock.Verify(r => r.GetByIdAsync(10),Times.Once());
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("Test", result.Name);
            Assert.Equal("test@gmail.com", result.Email);
            Assert.Equal("User", result.Role);
            Assert.False(result.IsActived);
        }
    }
}
