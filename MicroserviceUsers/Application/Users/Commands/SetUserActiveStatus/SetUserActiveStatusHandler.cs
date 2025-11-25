using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.SetUserActiveStatus
{
    public class SetUserActiveStatusCommandHandler(IUserRepository userRepository)
        : IRequestHandler<SetUserActiveStatusCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(
            SetUserActiveStatusCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден");

            user.IsActived = request.IsActive;

            await _userRepository.UpdateAsync(user.Id, user);
        }
    }
}
