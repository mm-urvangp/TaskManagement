using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories.IRepositories
{
    public interface ITaskAssignmentRepository
    {
        Task<IEnumerable<TaskAssignmentModel>> GetAllAsync();
        Task<IEnumerable<TaskAssignmentModel>> GetByUserIdAsync(int userId);
        Task<TaskAssignmentModel?> GetByIdAsync(int id);
        Task<TaskAssignmentModel> CreateAsync(TaskAssignmentModel model);
        Task<bool> DeleteAsync(int id);
    }
}
