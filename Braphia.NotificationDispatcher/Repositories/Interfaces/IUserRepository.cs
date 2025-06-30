using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(int userId, User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByIdAsync(int rootId, UserTypeEnum userType);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
