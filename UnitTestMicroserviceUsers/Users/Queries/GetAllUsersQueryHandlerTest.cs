using Application.Users.Queries.GetAllUsers;
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
    public class GetAllUsersQueryHandlerTest
    {
        [Fact]
        public async Task GetAllUsers_Should_Get_Users()
        {
            var repositoryUserMock = new Mock<IUserRepository>();
            var users = new List<User>
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
            repositoryUserMock.Setup(r => r.GetAllAsync(It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>())).ReturnsAsync(users);
            var handler = new GetAllUsersQueryHandler(repositoryUserMock.Object);
            var query = new GetAllUsersQuery(
                Name: null,
                Email: null,
                Role: null
            );

            var result = (await handler.Handle(query, CancellationToken.None)).ToList();

            repositoryUserMock.Verify(r =>
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
            var repositoryUserMock = new Mock<IUserRepository>();
            repositoryUserMock.Setup(r => r.GetAllAsync(It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>())).ReturnsAsync(new List<User>());
            var handler = new GetAllUsersQueryHandler(repositoryUserMock.Object);
            var query = new GetAllUsersQuery(
                Name: "User",
                Email: "user1@test.com",
                Role: "User"
            );

            await handler.Handle(query, CancellationToken.None);

            repositoryUserMock.Verify(r =>r.GetAllAsync("User", "user1@test.com", "User"),Times.Once);
        }
    }
}
