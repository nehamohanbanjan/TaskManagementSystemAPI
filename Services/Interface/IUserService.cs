using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.Services.Interface
{
    public interface IUserService
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User> RegisterUserAsync(RegisterUserDto dto);
        Task<List<User>> GetAllUsersAsync(int page, int pageSize);
        Task<User?> GetUserByIdAsync(int id);
    }
}
