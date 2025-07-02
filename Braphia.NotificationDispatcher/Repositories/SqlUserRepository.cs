using Braphia.NotificationDispatcher.Database;
using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.NotificationDispatcher.Repositories
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly ILogger<SqlUserRepository> _logger;
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlUserRepository(DBContext context, ILogger<SqlUserRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, User updatedUser)
        {
            try
            {
                var existingUser = await _context.User.FindAsync(userId);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", userId);
                    return false;
                }
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Email = updatedUser.Email;
                existingUser.UserType = updatedUser.UserType;
                _context.User.Update(existingUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found to delete", userId);
                    return false;
                }
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return false;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.User.FindAsync(userId);
        }

        public async Task<User?> GetUserByIdAsync(int rootId, UserTypeEnum userType)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.RootId == rootId && u.UserType == userType);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(UserTypeEnum? userType = null)
        {
            if (userType.HasValue)
            {
                return await _context.User
                    .Where(u => u.UserType == userType.Value)
                    .ToListAsync();
            }
            return await _context.User.ToListAsync();
        }
    }
}
