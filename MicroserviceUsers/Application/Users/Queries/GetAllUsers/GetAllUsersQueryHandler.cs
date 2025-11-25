using Application.dto.User;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler(IUserRepository userRepository)
        : IRequestHandler<GetAllUsersQuery, IEnumerable<ReadUserDto>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IEnumerable<ReadUserDto>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(
                request.Name, request.Email, request.Role);

            return users.Select(u => new ReadUserDto(
                u.Id,
                u.Name,
                u.Email,
                u.Role,
                u.IsActived
            ));
        }
    }
}
