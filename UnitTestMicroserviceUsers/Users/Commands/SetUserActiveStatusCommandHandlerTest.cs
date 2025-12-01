using Application.Users.Commands.SetUserActiveStatus;
using Domain.Interfaces;
using Domain.Models;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnitTestMicroserviceUsers.Users.Commands
{
    public class SetUserActiveStatusCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _repositoryUserMock;
        private readonly Mock<IHttpClientFactory> _httpFactoryMock;
        private readonly SetUserActiveStatusCommandHandler _handler;

        public SetUserActiveStatusCommandHandlerTest()
        {
           
            _repositoryUserMock = new Mock<IUserRepository>();
            _httpFactoryMock = new Mock<IHttpClientFactory>();
            _handler = new SetUserActiveStatusCommandHandler(_repositoryUserMock.Object, _httpFactoryMock.Object);
        }
        private SetUserActiveStatusCommand CreateCommand(int id, bool isActive)
        {
            return new SetUserActiveStatusCommand(Id: id, IsActive: isActive);
        }

        [Fact]
        public async Task SetUserActiveStatus_NotFound_User()
        {
            _repositoryUserMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

            var command = CreateCommand(1, true);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }
    }
}

