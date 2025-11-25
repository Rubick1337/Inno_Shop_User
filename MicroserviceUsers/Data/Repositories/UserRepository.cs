using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_3_Inno_PreTrainee.Data.Context;

namespace Data.Repositories
{
    public class UserRepository(UserContext context) : IUserRepository 
    {
        private readonly UserContext _context = context;
        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync(
                   string? name = null,
                   string? email = null,
                   string? role = null)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            if (name is not null)
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            if (email is not null)
            {
                query = query.Where(x => x.Email.Contains(email));
            }

            if (role is not null)
            {
                query = query.Where(x => x.Role == role);
            }

            return await query.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(int id, User updatedUser)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            updatedUser.Id = id;

            _context.Entry(existing).CurrentValues.SetValues(updatedUser);

            await _context.SaveChangesAsync();
        }
    }
}
