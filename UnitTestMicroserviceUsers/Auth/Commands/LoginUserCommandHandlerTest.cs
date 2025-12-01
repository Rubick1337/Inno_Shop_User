using Application.Auth.Commands.ConfirmEmail;
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
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<IJwtProvider>();

            _handler = new LoginUserCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtProviderMock.Object);
        }
        private LoginUserCommand CreateCommand(string email, string password)
        {
            return new LoginUserCommand(email, password);
        }

        [Fact]
        public async Task LoginUser_Should_Authorization()
        {
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
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(password, user.PasswordHash)).Returns(true);
            _jwtProviderMock.Setup(j => j.GenerateToken(user)).Returns("token");
            var command = CreateCommand(email, password);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("token", result);

            _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
            _passwordHasherMock.Verify(h => h.Verify(password, user.PasswordHash), Times.Once);
            _jwtProviderMock.Verify(j => j.GenerateToken(user), Times.Once);
        }

        [Fact]
        public async Task LoginUser_UserNotFound()
        {
            var email = "test@gmail.com";
            var password = "123456";
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var command = CreateCommand(email, password);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });

            _jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginUser_Wrong_Password()
        {
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
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(password, user.PasswordHash)).Returns(false);

            var command = CreateCommand(email, password);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async() =>
                await _handler.Handle(command, CancellationToken.None));

            _jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
