using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler(IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IRedisRepository redisRepository,
            IEmailService emailService,
            IJwtProvider jwtProvider,
            ICodeGenerator codeGenerator)
        : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IEmailService _emailService = emailService;
        private readonly IRedisRepository _redisRepository = redisRepository;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly ICodeGenerator _codeGenerator = codeGenerator;
        


        public async Task<string> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email);
            if (existing is not null)
                throw new InvalidOperationException("Пользователь уже существует");

            var hashedPassword = _passwordHasher.Generate(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = "User",
                IsActived = false
            };

            await _userRepository.CreateAsync(user);

            var confirmCode = _codeGenerator.GenerateCode();

            await _redisRepository.SetDataAsync(
                key: $"email_confirm:{request.Email}",
                value: confirmCode,
                minutes: 10);

            await _emailService.SendAsync(
                request.Email,
                "Подтверждение регистрации в InnoShop",
                $"<p>Для подтверждения вашего аккаунта введите код:</p>" +
                $"<p><b>{confirmCode}</b></p>"
            );

            var token = _jwtProvider.GenerateToken(user);
            return token;
        }
    }
}
