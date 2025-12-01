using Application.dto.User;
using Application.Users.Commands;
using Application.Users.Queries.GetAllUsers;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnitTestMicroserviceUsers.Users.Queries
{
    public class GetAllUsersQueryHandlerTest
    {
        private readonly Mock<IUserRepository> _repositoryUserMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTest()
        {
            _repositoryUserMock = new Mock<IUserRepository>();
            _handler = new GetAllUsersQueryHandler(_repositoryUserMock.Object);
        }

        private List<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "User1",
                    Email = "user1@test.com",
                    PasswordHash = "hash1",
                    Role = "User",
                    IsActived = false
                },
                new User
                {
                    Id = 2,
                    Name = "User2",
                    Email = "user2@test.com",
                    PasswordHash = "hash2",
                    Role = "Admin",
                    IsActived = true
                }
            };
        }
        private GetAllUsersQuery CreateQuery(string? name, string? email, string? role)
        {
            return new GetAllUsersQuery(name, email, role);
        }

        [Fact]
        public async Task GetAllUsers_Should_Get_Users()
        {
            var users = CreateTestUsers();
            _repositoryUserMock.Setup(r => r.GetAllAsync(It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>())).ReturnsAsync(users);
            var query = CreateQuery(null,null,null);

            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            _repositoryUserMock.Verify(r =>
               r.GetAllAsync(null, null, null), Times.Once);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("User1", result[0].Name);
            Assert.Equal(2, result[1].Id);
            Assert.Equal("User2", result[1].Name);
        }
        [Fact]
        public async Task GetAllUsers_Should_Pass_Filters()
        {
            _repositoryUserMock.Setup(r => r.GetAllAsync(It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>())).ReturnsAsync(new List<User>());
            var query = CreateQuery("User", "user1@test.com", "User");


            await _handler.Handle(query, CancellationToken.None);

            _repositoryUserMock.Verify(r =>r.GetAllAsync("User", "user1@test.com", "User"),Times.Once);
        }
    }
}
