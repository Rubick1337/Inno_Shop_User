using Application.Auth.Commands.ConfirmEmail;
using Application.Cachekeys;
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
    public class ConfirmEmailCommandHandlerTest
    {
        [Fact]
        public async Task ConfirmEmail_Should_Succesful()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var email = "test@gmail.com";
            var code = "123456";
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {email}";
            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Test",
                PasswordHash = "hash",
                Role = "User",
                IsActived = false
            };
            redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)) .ReturnsAsync(code);
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            var command = new ConfirmEmailCommand(email, code);
            var handler = new ConfirmEmailCommandHandler(userRepositoryMock.Object, redisRepositoryMock.Object);

            await handler.Handle(command, CancellationToken.None);

            userRepositoryMock.Verify(r =>r.UpdateAsync(user.Id, It.Is<User>(u => u.IsActived)),Times.Once);
            redisRepositoryMock.Verify(r =>r.RemoveAsync(key),Times.Once);
        }

        [Fact]
        public async Task ConfirmEmail_Wrong_Code()
        {
            var email = "test@gmail.com";
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {email}";
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync("654321");
            var handler = new ConfirmEmailCommandHandler(userRepositoryMock.Object, redisRepositoryMock.Object);
            var command = new ConfirmEmailCommand(email, Code: "123456");

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task ConfirmEmail_AlreadyActive_Should_DoNothing()
        {
            var email = "active@gmail.com";
            var code = "123456";
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {email}";
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Active",
                PasswordHash = "hash",
                Role = "User",
                IsActived = true 
            };
            redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync(code);
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            var handler = new ConfirmEmailCommandHandler(userRepositoryMock.Object, redisRepositoryMock.Object);
            var command = new ConfirmEmailCommand(email, code);

            await handler.Handle(command, CancellationToken.None);

            userRepositoryMock.Verify(r => r.UpdateAsync(1,user), Times.Never);
            redisRepositoryMock.Verify(r => r.RemoveAsync(code), Times.Never);
        }
    }
}
