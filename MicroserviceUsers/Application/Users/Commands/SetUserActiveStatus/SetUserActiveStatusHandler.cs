using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.SetUserActiveStatus
{
    public class SetUserActiveStatusCommandHandler(IUserRepository userRepository,
        IHttpClientFactory httpClientFactory)
        : IRequestHandler<SetUserActiveStatusCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task Handle(
            SetUserActiveStatusCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден");

            user.IsActived = request.IsActive;
            await _userRepository.UpdateAsync(user.Id, user);

            var client = _httpClientFactory.CreateClient("Products");
            var path = request.IsActive
                ? $"/api/products/{user.Id}/softundelete"
                : $"/api/products/{user.Id}/softdelete";

            var response = await client.PatchAsync(path, content: null, cancellationToken); 
            response.EnsureSuccessStatusCode();
        }
    }
}
