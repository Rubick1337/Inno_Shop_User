using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.SetUserActiveStatus;
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
        private readonly Mock<IUserRepository> _repositoryUserMock;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTest()
        {
            _repositoryUserMock = new Mock<IUserRepository>();
            _handler = new DeleteUserCommandHandler(_repositoryUserMock.Object);
        }
        private DeleteUserCommand CreateCommand(int id)
        {
            return new DeleteUserCommand(Id: id);
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
        [Fact]
        public async Task DeleteUser_Should_Delete_User()
        {
            var user = CreateTestUser();
            var command = CreateCommand(user.Id);

            await _handler.Handle(command, CancellationToken.None);

            _repositoryUserMock.Verify(r => r.DeleteAsync(10), Times.Once());
        }
    }
}
