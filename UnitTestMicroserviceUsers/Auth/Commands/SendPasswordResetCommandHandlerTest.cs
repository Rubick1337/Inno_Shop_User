using Application.Auth.Commands.SendEmailConfirmation;
using Application.Auth.Commands.SendPasswordReset;
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
    public class SendPasswordResetCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRedisRepository> _redisRepositoryMock;
        private readonly Mock<ICodeGenerator> _codeGeneratorMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly SendPasswordResetCommandHandler _handler;

        public SendPasswordResetCommandHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _redisRepositoryMock = new Mock<IRedisRepository>();
            _codeGeneratorMock = new Mock<ICodeGenerator>();
            _emailServiceMock = new Mock<IEmailService>();

            _handler = new SendPasswordResetCommandHandler(
                _userRepositoryMock.Object,
                _redisRepositoryMock.Object,
                _codeGeneratorMock.Object,
                _emailServiceMock.Object);
        }
        private SendPasswordResetCommand CreateCommand(string email)
        {
            return new SendPasswordResetCommand(email);
        }

        [Fact]
        public async Task Handle_SendPasswordReset_Should_SendEmail()
        {
            var email = "test@example.com";
            var key = $"{CacheKeys.PASSWORD_RESET} + {email}";
            var generatedCode = "123456";

            var user = new User
            {
                Id = 1,
                Email = email,
                Name = "Test",
                PasswordHash = "hash",
                Role = "User",
                IsActived = true
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _codeGeneratorMock.Setup(c => c.GenerateCode(6)).Returns(generatedCode);

            var command = CreateCommand(email);
            await _handler.Handle(command, CancellationToken.None);

            _redisRepositoryMock.Verify(r =>r.SetDataAsync(key,generatedCode,15),Times.Once);
            _emailServiceMock.Verify(e =>
                    e.SendAsync(
                        email,
                        "Восстановление пароля",
                        $"Ваш код восстановления:<br>{generatedCode}"),Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound()
        {
            var email = "notfound@example.com";
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var command = CreateCommand(email);
            await _handler.Handle(command, CancellationToken.None);

            _redisRepositoryMock.Verify(r => r.SetDataAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<int>()),
                Times.Never);

            _codeGeneratorMock.Verify(c => c.GenerateCode(6), Times.Never);
            _emailServiceMock.Verify(e =>
                    e.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Never);
        }
    }
}

