using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories.IRepositories;

namespace TaskManagement.API.Repositories
{
    public class TaskAssignmentRepository: ITaskAssignmentRepository
    {
        private readonly AppDbContext _context;

        public TaskAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskAssignmentModel>> GetAllAsync()
        {
            return await _context.TaskAssignment
                .Include(x => x.Task)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskAssignmentModel>> GetByUserIdAsync(int userId)
        {
            return await _context.TaskAssignment
                .Include(x => x.Task)
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<TaskAssignmentModel?> GetByIdAsync(int id)
        {
            return await _context.TaskAssignment
                .Include(x => x.Task)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TaskAssignmentModel> CreateAsync(TaskAssignmentModel model)
        {
            await _context.TaskAssignment.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.TaskAssignment.FindAsync(id);
            if (entity == null) return false;

            _context.TaskAssignment.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
