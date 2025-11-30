using Application.dto.User;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetUserByIdQuery, ReadUserDto?>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<ReadUserDto?> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
                return null;

            return new ReadUserDto(
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.IsActived
            );
        }
    }
}
