using Application.Users.Commands.DeleteUser;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMicroserviceUsers.Users.Commands
{
    public class DeleteUserCommandHandlerTest
    {
        [Fact]
        public async Task DeleteUser_Should_Delete_User()
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
            var handler = new DeleteUserCommandHandler(repositoryUserMock.Object);
            var command = new DeleteUserCommand(Id: 10);

            await handler.Handle(command, CancellationToken.None);

            repositoryUserMock.Verify(r => r.DeleteAsync(10), Times.Once());
        }
    }
}
