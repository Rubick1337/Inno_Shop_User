using Application.Auth.Commands.LoginUser;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMicroserviceUsers.Auth.Commands
{
    public class LoginUserCommandHandlerTest
    {
        [Fact]
        public async Task LoginUser_Should_Authorization()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtProviderMock = new Mock<IJwtProvider>();
            var email = "test@gmail.com";
            var password = "123456";
            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Test",
                PasswordHash = "hashed_pwd",
                Role = "User",
                IsActived = true
            };
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            passwordHasherMock.Setup(h => h.Verify(password, user.PasswordHash)).Returns(true);
            jwtProviderMock.Setup(j => j.GenerateToken(user)).Returns("token");
            var handler = new LoginUserCommandHandler(userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtProviderMock.Object);
            var command = new LoginUserCommand(email, password);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("token", result);

            userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
            passwordHasherMock.Verify(h => h.Verify(password, user.PasswordHash), Times.Once);
            jwtProviderMock.Verify(j => j.GenerateToken(user), Times.Once);
        }

        [Fact]
        public async Task LoginUser_UserNotFound()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtProviderMock = new Mock<IJwtProvider>();
            var email = "test@gmail.com";
            var password = "123456";
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            var handler = new LoginUserCommandHandler(userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtProviderMock.Object);
            var command = new LoginUserCommand(email, password);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);

        }

        [Fact]
        public async Task LoginUser_Wrong_Password()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var jwtProviderMock = new Mock<IJwtProvider>();
            var email = "test@gmail.com";
            var password = "123456";
            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Test",
                PasswordHash = "hashed_pwd",
                Role = "User",
                IsActived = true
            };
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            passwordHasherMock.Setup(h => h.Verify(password, user.PasswordHash)).Returns(false);
            var handler = new LoginUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                jwtProviderMock.Object);
            var command = new LoginUserCommand(email, password);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async() =>
                await handler.Handle(command, CancellationToken.None));

            jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
