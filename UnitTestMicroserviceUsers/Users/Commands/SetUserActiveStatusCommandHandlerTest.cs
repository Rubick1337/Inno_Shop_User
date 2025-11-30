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
    public class SetUserActiveStatusCommandHandlerTest()
    {
        [Fact]
        public async Task SetUserActiveStatus_NotFound_User()
        {
            var repositoryUserMock = new Mock<IUserRepository>();
            var httpFactoryMock = new Mock<IHttpClientFactory>();

            repositoryUserMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);
            var handler = new SetUserActiveStatusCommandHandler(repositoryUserMock.Object,httpFactoryMock.Object);
            var command = new SetUserActiveStatusCommand(Id: 1, IsActive: true);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}

