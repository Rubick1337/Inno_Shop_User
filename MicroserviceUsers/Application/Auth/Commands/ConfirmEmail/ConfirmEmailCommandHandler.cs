using Application.Cachekeys;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler(IUserRepository userRepository,
           IRedisRepository redisRepository)
        : IRequestHandler<ConfirmEmailCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRedisRepository _redisRepository = redisRepository;

        public async Task Handle(
            ConfirmEmailCommand request,
            CancellationToken cancellationToken)
        {
            var key = $"{CacheKeys.EMAIL_CONFIRM} + {request.Email}";
            var storedCode = await _redisRepository.GetDataAsync<string>(key);

            if (storedCode is null || storedCode != request.Code)
                throw new UnauthorizedAccessException("Неверный или истёкший код");

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                throw new InvalidOperationException("Пользователь не найден");

            if (user.IsActived)
                return;

            user.IsActived = true;
            await _userRepository.UpdateAsync(user.Id, user);

            await _redisRepository.RemoveAsync(key);
        }
    }
}
