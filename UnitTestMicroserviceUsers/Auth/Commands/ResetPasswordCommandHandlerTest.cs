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
        [Fact]
        public async Task ResetPassword_Should_Update()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
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

            redisRepositoryMock .Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync("123456");
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            passwordHasherMock.Setup(h => h.Generate(newPassword)).Returns("new_hash");

            var handler = new ResetPasswordCommandHandler(
                userRepositoryMock.Object,
                redisRepositoryMock.Object,
                passwordHasherMock.Object);

            var command = new ResetPasswordCommand(email,"123456", newPassword);

            await handler.Handle(command, CancellationToken.None);

            redisRepositoryMock.Verify(r => r.GetDataAsync<string>(key), Times.Once);
            passwordHasherMock.Verify(h => h.Generate(newPassword), Times.Once);

            userRepositoryMock.Verify(r =>r.UpdateAsync(user.Id, It.Is<User>(u => u.PasswordHash == "new_hash")),Times.Once);
        }

        [Fact]
        public async Task ResetPassword_WhenUserNotFound()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var email = "test@example.com";
            var key = $"{CacheKeys.PASSWORD_RESET} + {email}";

            redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync("123456");
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var handler = new ResetPasswordCommandHandler(
                userRepositoryMock.Object,
                redisRepositoryMock.Object,
                passwordHasherMock.Object);

            var command = new ResetPasswordCommand(email,"new_password","123456");

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await handler.Handle(command, CancellationToken.None));

            passwordHasherMock.Verify(h => h.Generate(It.IsAny<string>()), Times.Never);
            userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<User>()), Times.Never);
        }
    }
}
