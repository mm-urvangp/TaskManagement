using TaskManagement.API.Models.DTOs;

namespace TaskManagement.API.Repositories.IRepositories
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
