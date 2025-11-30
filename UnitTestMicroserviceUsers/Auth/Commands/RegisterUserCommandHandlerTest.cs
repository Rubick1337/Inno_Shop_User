using Application.Auth.Commands.RegisterUser;
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
    public class RegisterUserCommandHandlerTest
    {
        [Fact]
        public async Task RegisterUser_Should_CreateUser()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var emailServiceMock = new Mock<IEmailService>();
            var jwtProviderMock = new Mock<IJwtProvider>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();
            var name = "TestUser";
            var email = "test@example.com";
            var password = "123456";
            var expectedKey = $"{CacheKeys.EMAIL_CONFIRM} + {email}";
            userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            passwordHasherMock.Setup(h => h.Generate(password)).Returns("hashed");
            codeGeneratorMock.Setup(c => c.GenerateCode(6)).Returns("123456");
            jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");
            var handler = new RegisterUserCommandHandler(userRepositoryMock.Object,
                passwordHasherMock.Object,
                redisRepositoryMock.Object,
                emailServiceMock.Object,
                jwtProviderMock.Object,
                codeGeneratorMock.Object);
            var command = new RegisterUserCommand(name,email,password);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("token", result);

            emailServiceMock.Verify(e => e.SendAsync(email,
                "Подтверждение регистрации в InnoShop",
                It.Is<string>(body => body.Contains("123456"))),Times.Once);
            jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Once);
            redisRepositoryMock.Verify(r => r.SetDataAsync( expectedKey,"123456",10), Times.Once);
            passwordHasherMock.Verify(h => h.Generate(password), Times.Once);
            userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_WhenUserAlreadyExists()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var passwordHasherMock = new Mock<IPasswordHasher>();
            var redisRepositoryMock = new Mock<IRedisRepository>();
            var emailServiceMock = new Mock<IEmailService>();
            var jwtProviderMock = new Mock<IJwtProvider>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();

            var email = "test@example.com";

            userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
               .ReturnsAsync(new User
                {
                    Id = 1,
                    Email = email,
                    Name = "User",
                    PasswordHash = "hash",
                    Role = "User",
                    IsActived = true
                });

            var handler = new RegisterUserCommandHandler(
                userRepositoryMock.Object,
                passwordHasherMock.Object,
                redisRepositoryMock.Object,
                emailServiceMock.Object,
                jwtProviderMock.Object,
                codeGeneratorMock.Object);
            var command = new RegisterUserCommand("NewUser",email,"123456");

            await Assert.ThrowsAsync<InvalidOperationException>(async() =>
                await handler.Handle(command, CancellationToken.None));

            userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
            jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
