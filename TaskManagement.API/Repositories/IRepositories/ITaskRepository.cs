using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories.IRepositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskModel>> GetAllAsync();
        Task<TaskModel> GetByIdAsync(int id);
        Task<TaskModel> AddAsync(TaskModel task);
        Task<bool> UpdateAsync(TaskModel task);
        Task<bool> DeleteAsync(int id);
    }
}
