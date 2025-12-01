using Application.dto.User;
using Application.Users.Commands;
using Application.Users.Commands.DeleteUser;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnitTestMicroserviceUsers.Users.Commands
{
    public class UpdateUserCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _repositoryUserMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTest()
        {
            _repositoryUserMock = new Mock<IUserRepository>();
            _handler = new UpdateUserCommandHandler(_repositoryUserMock.Object);
        }
        private UpdateUserCommand CreateCommand(int id,UpdateUserDto dto)
        {
            return new UpdateUserCommand(Id: id, Dto: dto);
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
        public async Task UpdateUser_Should_Update_User()
        {
            var user = CreateTestUser();
            var userDto = new UpdateUserDto(Name: "Test2",
                Email: "test2@gmail.com",Role: "Admin");
            _repositoryUserMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);

            var command = CreateCommand(10,userDto);

            await _handler.Handle(command, CancellationToken.None);

            _repositoryUserMock.Verify(r => r.UpdateAsync(10, It.Is<User>(u => u.Name == "Test2" 
            && u.Email == "test2@gmail.com")), Times.Once());
        }
        [Fact]
        public async Task UpdateUser_NotFound_User()
        {
            _repositoryUserMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((User?)null);
            var userDto = new UpdateUserDto(Name: "Test2",
                Email: "test2@gmail.com", Role: "Admin");

            var command = CreateCommand(10, userDto);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }
    }
}
