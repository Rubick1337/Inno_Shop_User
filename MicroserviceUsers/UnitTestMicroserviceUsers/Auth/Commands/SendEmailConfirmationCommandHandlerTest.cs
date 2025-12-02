using Application.Auth.Commands.ResetPassword;
using Application.Auth.Commands.SendEmailConfirmation;
using Application.Cachekeys;
using Application.Interfaces;
using Application.Services;
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
    public class SendEmailConfirmationCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRedisRepository> _redisRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ICodeGenerator> _codeGeneratorMock;
        private readonly SendEmailConfirmationCommandHandler _handler;

        public SendEmailConfirmationCommandHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _redisRepositoryMock = new Mock<IRedisRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _codeGeneratorMock = new Mock<ICodeGenerator>();

            _handler = new SendEmailConfirmationCommandHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _redisRepositoryMock.Object,
                _codeGeneratorMock.Object);
        }

        private SendEmailConfirmationCommand CreateCommand(string email)
        {
            return new SendEmailConfirmationCommand(email);
        }

        [Fact]
        public async Task Handle_UserNotFound_Should_DoNothing()
        {
            var email = "test@example.com";
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            var command = CreateCommand(email);

            await _handler.Handle(command, CancellationToken.None);

            _redisRepositoryMock.Verify(r => r.SetDataAsync( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),Times.Never);
            _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        [Fact]
        public async Task Handle_UserAlreadyActive_Should_DoNothing()
        {
            var email = "active@example.com";
            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Active",
                PasswordHash = "hash",
                Role = "User",
                IsActived = true
            };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            var command = CreateCommand(email);
            await _handler.Handle(command, CancellationToken.None);

            _redisRepositoryMock.Verify(r => r.SetDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),Times.Never);
            _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }
        [Fact]
        public async Task Handle_InactiveUser_Should_SetCodeInRedis_And_SendEmail()
        {
            var email = "user@example.com";
            var code = "123456";
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {email}";

            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "User",
                PasswordHash = "hash",
                Role = "User",
                IsActived = false
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _codeGeneratorMock.Setup(g => g.GenerateCode(6)).Returns(code);

            var command = CreateCommand(email);
            await _handler.Handle(command, CancellationToken.None);

            _redisRepositoryMock.Verify(r =>r.SetDataAsync(key, code, 15),Times.Once);
            _emailServiceMock.Verify(e =>
                e.SendAsync( email,
                "Повторная отправка подтверждения Email",
                It.Is<string>(body => body.Contains(code))),
                Times.Once);
        }
    }
}
