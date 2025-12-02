using Application.Auth.Commands.RegisterUser;
using Application.Auth.Commands.ResetPassword;
using Application.Cachekeys;
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
    public class ResetPasswordCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRedisRepository> _redisRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly ResetPasswordCommandHandler _handler;

        public ResetPasswordCommandHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _redisRepositoryMock = new Mock<IRedisRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();

            _handler = new ResetPasswordCommandHandler(
                _userRepositoryMock.Object,
                _redisRepositoryMock.Object,
                _passwordHasherMock.Object);
        }
        private ResetPasswordCommand CreateCommand(string email, string code, string password)
        {
            return new ResetPasswordCommand(email, code, password);
        }

        [Fact]
        public async Task ResetPassword_Should_Update()
        {

            var email = "test@example.com";
            var newPassword = "new_password";
            var key = $"{CacheKeys.PASSWORD_RESET} + {email}";

            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Test",
                PasswordHash = "old_hash",                                                                                                                                                                                                                                       
                Role = "User",
                IsActived = true
            };

            _redisRepositoryMock .Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync("123456");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Generate(newPassword)).Returns("new_hash");


            var command = CreateCommand(email,"123456", newPassword);

            await _handler.Handle(command, CancellationToken.None);

            _redisRepositoryMock.Verify(r => r.GetDataAsync<string>(key), Times.Once);
            _passwordHasherMock.Verify(h => h.Generate(newPassword), Times.Once);

            _userRepositoryMock.Verify(r =>r.UpdateAsync(user.Id, It.Is<User>(u => u.PasswordHash == "new_hash")),Times.Once);
        }

        [Fact]
        public async Task ResetPassword_WhenUserNotFound()
        {
            var email = "test@example.com";
            var key = $"{CacheKeys.PASSWORD_RESET} + {email}";

            _redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync("123456");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var command = CreateCommand(email,"new_password","123456");

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            _passwordHasherMock.Verify(h => h.Generate(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<User>()), Times.Never);
        }
    }
}
