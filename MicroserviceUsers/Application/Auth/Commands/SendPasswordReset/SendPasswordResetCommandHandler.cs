using Application.Cachekeys;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.SendPasswordReset
{
    public class SendPasswordResetCommandHandler(IUserRepository userRepository,
            IRedisRepository redisRepository,
            ICodeGenerator codeGenerator,
            IEmailService emailService)
        : IRequestHandler<SendPasswordResetCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRedisRepository _redisRepository = redisRepository;
        private readonly ICodeGenerator _codeGenerator = codeGenerator;
        private readonly IEmailService _emailService = emailService;

        public async Task Handle(
            SendPasswordResetCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            var key = $"{CacheKeys.PASSWORD_RESET} + {request.Email}";

            if (user is null)
                return;

            var code = _codeGenerator.GenerateCode();

            await _redisRepository.SetDataAsync(key,
                value: code,
                minutes: 15);

            await _emailService.SendAsync(
                request.Email,
                "Восстановление пароля",
                $"Ваш код восстановления:<br>{code}"
            );
        }
    }
}
