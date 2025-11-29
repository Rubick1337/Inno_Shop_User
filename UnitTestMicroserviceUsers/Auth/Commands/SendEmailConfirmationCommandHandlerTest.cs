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
        [Fact]
        public async Task Handle_UserNotFound_Should_DoNothing()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var emailServiceMock = new Mock<IEmailService>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();

            var email = "test@example.com";

            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var handler = new SendEmailConfirmationCommandHandler(
                userRepositoryMock.Object,
                emailServiceMock.Object,
                redisRepositoryMock.Object,
                codeGeneratorMock.Object);

            var command = new SendEmailConfirmationCommand(Email: email);

            await handler.Handle(command, CancellationToken.None);

            redisRepositoryMock.Verify(r => r.SetDataAsync( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),Times.Never);
            emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        [Fact]
        public async Task Handle_UserAlreadyActive_Should_DoNothing()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var emailServiceMock = new Mock<IEmailService>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();

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
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            var handler = new SendEmailConfirmationCommandHandler(
                userRepositoryMock.Object,
                emailServiceMock.Object,
                redisRepositoryMock.Object,
                codeGeneratorMock.Object);

            var command = new SendEmailConfirmationCommand(Email: email);

            await handler.Handle(command, CancellationToken.None);

            redisRepositoryMock.Verify(r => r.SetDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),Times.Never);
            emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }
        [Fact]
        public async Task Handle_InactiveUser_Should_SetCodeInRedis_And_SendEmail()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var emailServiceMock = new Mock<IEmailService>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();

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

            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            codeGeneratorMock.Setup(g => g.GenerateCode(6)).Returns(code);

            var handler = new SendEmailConfirmationCommandHandler(
                userRepositoryMock.Object,
                emailServiceMock.Object,
                redisRepositoryMock.Object,
                codeGeneratorMock.Object);

            var command = new SendEmailConfirmationCommand(Email: email);

            await handler.Handle(command, CancellationToken.None);

            redisRepositoryMock.Verify(r =>r.SetDataAsync(key, code, 15),Times.Once);
            emailServiceMock.Verify(e =>
                e.SendAsync( email,
                "Повторная отправка подтверждения Email",
                It.Is<string>(body => body.Contains(code))),
                Times.Once);
        }
    }
}
