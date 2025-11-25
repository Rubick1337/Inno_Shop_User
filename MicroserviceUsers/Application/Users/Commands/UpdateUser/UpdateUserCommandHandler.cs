using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class UpdateUserCommandHandler(IUserRepository userRepository)
        : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByIdAsync(request.Id);

            if (existing is null)
                throw new InvalidOperationException("Пользователь не найден");

            var dto = request.Dto;

            if (dto.Name is not null)
                existing.Name = dto.Name;

            if (dto.Email is not null)
                existing.Email = dto.Email;

            await _userRepository.UpdateAsync(request.Id, existing);
        }
    }
}
