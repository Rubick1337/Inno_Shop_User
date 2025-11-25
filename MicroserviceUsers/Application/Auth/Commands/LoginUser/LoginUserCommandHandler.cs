using Application.Interfaces;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.LoginUser
{
    public class LoginUserCommandHandler(IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider)
        : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtProvider _jwtProvider = jwtProvider;


        public async Task<string> Handle(
            LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user is null)
                throw new UnauthorizedAccessException("Неверный email или пароль");

            var ok = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!ok)
                throw new UnauthorizedAccessException("Неверный email или пароль");

            return _jwtProvider.GenerateToken(user);
        }
    }
}
