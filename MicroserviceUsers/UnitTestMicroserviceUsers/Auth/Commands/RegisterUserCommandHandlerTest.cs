using Application.Auth.Commands.LoginUser;
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
using System.Xml.Linq;

namespace UnitTestMicroserviceUsers.Auth.Commands
{
    public class RegisterUserCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IRedisRepository> _redisRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly Mock<ICodeGenerator> _codeGeneratorMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _redisRepositoryMock = new Mock<IRedisRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _codeGeneratorMock = new Mock<ICodeGenerator>();

            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _redisRepositoryMock.Object,
                _emailServiceMock.Object,
                _jwtProviderMock.Object,
                _codeGeneratorMock.Object);
        }
        private RegisterUserCommand CreateCommand(string name, string email, string password)
        {
            return new RegisterUserCommand(name, email, password);
        }

        [Fact]
        public async Task RegisterUser_Should_CreateUser()
        {

            var name = "TestUser";
            var email = "test@example.com";
            var password = "123456";
            var expectedKey = $"{CacheKeys.EMAIL_CONFIRM} + {email}";

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(h => h.Generate(password)).Returns("hashed");
            _codeGeneratorMock.Setup(c => c.GenerateCode(6)).Returns("123456");
            _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

            var command = CreateCommand( name, email, password);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("token", result);

            _emailServiceMock.Verify(e => e.SendAsync(email,
                "Подтверждение регистрации в InnoShop",
                It.Is<string>(body => body.Contains("123456"))),Times.Once);
            _jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Once);
            _redisRepositoryMock.Verify(r => r.SetDataAsync( expectedKey,"123456",10), Times.Once);
            _passwordHasherMock.Verify(h => h.Generate(password), Times.Once);
            _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_WhenUserAlreadyExists()
        {
            var email = "test@example.com";

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email))
               .ReturnsAsync(new User
                {
                    Id = 1,
                    Email = email,
                    Name = "User",
                    PasswordHash = "hash",
                    Role = "User",
                    IsActived = true
                });
            var command = CreateCommand("NewUser", email, "123456");

            await Assert.ThrowsAsync<InvalidOperationException>(async() =>
                await _handler.Handle(command, CancellationToken.None));

            _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
            _jwtProviderMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
