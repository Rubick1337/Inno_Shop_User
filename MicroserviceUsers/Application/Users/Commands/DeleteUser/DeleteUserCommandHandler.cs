using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(IUserRepository userRepository)
        : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            await _userRepository.DeleteAsync(request.Id);
        }
    }
}
