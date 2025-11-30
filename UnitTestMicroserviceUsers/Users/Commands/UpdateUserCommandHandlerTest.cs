using Application.dto.User;
using Application.Users.Commands;
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
        [Fact]
        public async Task UpdateUser_Should_Update_User()
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
            var userDto = new UpdateUserDto(Name: "Test2",
                Email: "test2@gmail.com",Role: "Admin");
            repositoryUserMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(user);
            var command = new UpdateUserCommand(Id: 10, Dto: userDto);
            var handler = new UpdateUserCommandHandler(repositoryUserMock.Object);

            await handler.Handle(command, CancellationToken.None);

            repositoryUserMock.Verify(r => r.UpdateAsync(10, It.Is<User>(u => u.Name == "Test2" && u.Email == "test2@gmail.com")), Times.Once());
        }
        [Fact]
        public async Task UpdateUser_NotFound_User()
        {
            var repositoryUserMock = new Mock<IUserRepository>();
            repositoryUserMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((User?)null);
            var userDto = new UpdateUserDto(Name: "Test2",
                Email: "test2@gmail.com", Role: "Admin");
            var command = new UpdateUserCommand(Id: 10, Dto: userDto);
            var handler = new UpdateUserCommandHandler(repositoryUserMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}
