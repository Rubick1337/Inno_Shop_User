using Application.Auth.Commands.ConfirmEmail;
using Application.Cachekeys;
using Application.Users.Commands.DeleteUser;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace UnitTestMicroserviceUsers.Auth.Commands
{
    public class ConfirmEmailCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRedisRepository> _redisRepositoryMock;
        private readonly ConfirmEmailCommandHandler _handler;

        public ConfirmEmailCommandHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _redisRepositoryMock = new Mock<IRedisRepository>();
            _handler = new ConfirmEmailCommandHandler(
                _userRepositoryMock.Object,
                _redisRepositoryMock.Object);
        }
        private ConfirmEmailCommand CreateCommand(string email, string code)
        {
            return new ConfirmEmailCommand(email, code);
        }
        [Fact]
        public async Task ConfirmEmail_Should_Succesful()
        {
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
            _redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)) .ReturnsAsync(code);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            var command = CreateCommand(email, code);

            await _handler.Handle(command, CancellationToken.None);

            _userRepositoryMock.Verify(r =>r.UpdateAsync(user.Id, It.Is<User>(u => u.IsActived)),Times.Once);
            _redisRepositoryMock.Verify(r =>r.RemoveAsync(key),Times.Once);
        }

        [Fact]
        public async Task ConfirmEmail_Wrong_Code()
        {
            var email = "test@gmail.com";
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {email}";
            var code = "123456";
            _redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync("654321");
            var command = CreateCommand(email, code);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task ConfirmEmail_AlreadyActive_Should_DoNothing()
        {
            var email = "active@gmail.com";
            var code = "123456";
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {email}";
            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Active",
                PasswordHash = "hash",
                Role = "User",
                IsActived = true 
            };
            _redisRepositoryMock.Setup(r => r.GetDataAsync<string>(key)).ReturnsAsync(code);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            var command = CreateCommand(email, code);

            await _handler.Handle(command, CancellationToken.None);

            _userRepositoryMock.Verify(r => r.UpdateAsync(1,user), Times.Never);
            _redisRepositoryMock.Verify(r => r.RemoveAsync(code), Times.Never);
        }
    }
}
