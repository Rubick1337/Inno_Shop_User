using Application.Users.Commands.SetUserActiveStatus;
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

namespace UnitTestMicroserviceUsers.Users.Commands
{
    public class SetUserActiveStatusCommandHandlerTest()
    {
        [Fact]
        public async Task SetUserActiveStatus_Should_Update_IsActived()
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
            var handler = new SetUserActiveStatusCommandHandler(repositoryUserMock.Object);
            var command = new SetUserActiveStatusCommand(Id: 10, IsActive: true);

            await handler.Handle(command, CancellationToken.None);

            repositoryUserMock.Verify(r => r.UpdateAsync(10,
                It.Is<User>(u => u.IsActived == true)));
        }

        [Fact]
        public async Task SetUserActiveStatus_NotFound_User()
        {
            var repositoryUserMock = new Mock<IUserRepository>();

            repositoryUserMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);
            var handler = new SetUserActiveStatusCommandHandler(repositoryUserMock.Object);
            var command = new SetUserActiveStatusCommand(Id: 1, IsActive: true);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}

