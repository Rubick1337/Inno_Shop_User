using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandHandler(IUserRepository userRepository,
            IEmailService emailService, 
            IRedisRepository redisRepository,
            ICodeGenerator codeGenerator)
        : IRequestHandler<SendEmailConfirmationCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRedisRepository _redisRepository = redisRepository;
        private readonly ICodeGenerator _codeGenerator = codeGenerator;
        private readonly IEmailService _emailService = emailService;

        public async Task Handle(
            SendEmailConfirmationCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null || user.IsActived)
                return;

            var code = _codeGenerator.GenerateCode();

            await _redisRepository.SetDataAsync(
                key: $"email_confirm:{request.Email}",
                value: code,
                minutes: 15);

            await _emailService.SendAsync(
                request.Email,
                "Повторная отправка подтверждения Email",
                $"<p>Чтобы подтвердить ваш аккаунт в InnoShop, введите этот код:</p>" +
                $"<p><b>{code}</b></p>"
            );
        }
    }
}
