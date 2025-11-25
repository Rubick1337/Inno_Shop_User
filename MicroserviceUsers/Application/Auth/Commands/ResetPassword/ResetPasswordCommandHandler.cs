using Application.Interfaces;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(IUserRepository userRepository,
            IRedisRepository redisRepository,
            IPasswordHasher passwordHasher)
        : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRedisRepository _redisRepository = redisRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var key = $"pwd_reset:{request.Email}";

            var storedCode = await _redisRepository.GetDataAsync<string>(key);

            if (storedCode is null)
            {
                throw new UnauthorizedAccessException("Неверный или истёкший код");
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден");

            user.PasswordHash = _passwordHasher.Generate(request.NewPassword);

            await _userRepository.UpdateAsync(user.Id, user);

            await _redisRepository.RemoveAsync(key);
        }
    }
}
