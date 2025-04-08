using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories.IRepositories;

namespace TaskManagement.API.Repositories
{
    public class TaskRepository: ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskModel> AddAsync(TaskModel task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(TaskModel task)
        {
            _context.Tasks.Update(task);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
