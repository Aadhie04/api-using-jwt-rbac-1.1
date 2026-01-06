using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserModel>> GetAllUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> CreateUserAsync(UserModel user);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(int id);
        Task<List<UserModel>> GetPendingUsersAsync();
        Task<List<UserModel>> GetApprovedUsersAsync();
        Task<bool> ApproveUserAsync(int id);
        Task<bool> RejectUserAsync(int id);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<UserModel?> GetByRefreshTokenHashAsync(string refreshTokenHash);
        Task<bool> UpdateUserRefreshTokenAsync(int userId, string? refreshTokenHash, DateTime? expiry);
        Task<List<string>> GetUserPermissionAsync(int userid);
        Task AssignUserRoleAsync(int userId, int roleId);
        Task<int> GetRoleIdByName(string roleName);
        Task<string> GetUserRoleNameAsync(int userId);

    }
}
