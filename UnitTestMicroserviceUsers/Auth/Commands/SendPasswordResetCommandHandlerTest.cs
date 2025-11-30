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
        [Fact]
        public async Task Handle_SendPasswordReset_Should_SendEmail()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();
            var emailServiceMock = new Mock<IEmailService>();

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

            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            codeGeneratorMock.Setup(c => c.GenerateCode(6)).Returns(generatedCode);

            var handler = new SendPasswordResetCommandHandler(
                userRepositoryMock.Object,
                redisRepositoryMock.Object,
                codeGeneratorMock.Object,
                emailServiceMock.Object);
            var command = new SendPasswordResetCommand(email);
            await handler.Handle(command, CancellationToken.None);

            redisRepositoryMock.Verify(r =>r.SetDataAsync(key,generatedCode,15),Times.Once);
            emailServiceMock.Verify(e =>
                    e.SendAsync(
                        email,
                        "Восстановление пароля",
                        $"Ваш код восстановления:<br>{generatedCode}"),Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();
            var emailServiceMock = new Mock<IEmailService>();

            var email = "notfound@example.com";

            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var handler = new SendPasswordResetCommandHandler(
                userRepositoryMock.Object,
                redisRepositoryMock.Object,
                codeGeneratorMock.Object,
                emailServiceMock.Object);
            var command = new SendPasswordResetCommand(email);
            await handler.Handle(command, CancellationToken.None);

            redisRepositoryMock.Verify(r => r.SetDataAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<int>()),
                Times.Never);

            codeGeneratorMock.Verify(c => c.GenerateCode(6), Times.Never);
            emailServiceMock.Verify(e =>
                    e.SendAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                Times.Never);
        }
    }
}

