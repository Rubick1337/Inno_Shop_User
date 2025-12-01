using Application.Users.Queries.GetAllUsers;
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
        private readonly Mock<IUserRepository> _repositoryUserMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTest()
        {
            _repositoryUserMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_repositoryUserMock.Object);
        }
        private User CreateTestUser()
        {
            return new User
            {
                Id = 10,
                Name = "Test",
                Email = "test@gmail.com",
                PasswordHash = "hash",
                Role = "User",
                IsActived = false
            };
        }
        private GetUserByIdQuery CreateQuery(int id)
        {
            return new GetUserByIdQuery(Id: id);
        }
        [Fact]
        public async Task GetUsersById_Should_Get_User()
        {
            var user = CreateTestUser();
            _repositoryUserMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);
            var query = CreateQuery(10);

            var result = await _handler.Handle(query,CancellationToken.None);

            _repositoryUserMock.Verify(r => r.GetByIdAsync(10),Times.Once());
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("Test", result.Name);
            Assert.Equal("test@gmail.com", result.Email);
            Assert.Equal("User", result.Role);
            Assert.False(result.IsActived);
        }
    }
}
