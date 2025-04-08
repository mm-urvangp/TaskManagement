using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagement.API.Data;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories.IRepositories;

namespace TaskManagement.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserModel?> GetByMobileAsync(string mobile)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Mobile == mobile);
        }

        public async Task<UserModel> CreateAsync(UserModel model)
        {
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> UpdateAsync(UserModel updated)
        {
            _context.Users.Update(updated);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
