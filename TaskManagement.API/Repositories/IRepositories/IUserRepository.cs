using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetAllAsync();
        Task<UserModel?> GetByIdAsync(int id);
        Task<UserModel?> GetByEmailAsync(string email);
        Task<UserModel?> GetByMobileAsync(string mobile);
        Task<UserModel> CreateAsync(UserModel user);
        Task<bool> UpdateAsync(UserModel user);
        Task<bool> DeleteAsync(int id);
    }
}
