using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> CreateUserAsync(UserModel user);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(int userid, string performedBy, string role);
        Task<List<UserModel>> GetPendingUsersAsync();
        Task<List<UserModel>> GetApprovedUsersAsync();
        Task<bool> ApproveUserAsync(int id, string performedBy, string role);
        Task<bool> RejectUserAsync(int id, string performedBy, string role);
        Task<UserModel> ValidateLoginAsync(string username, string password);
        Task<UserModel?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateUserRefreshTokenAsync(int userId, string refreshTokenHash, DateTime expiry);
        Task<bool> RevokeUserRefreshTokenAsync(int userId);
        Task<List<string>> GetUserPermissionAsync(int userId);
        Task<string> GetUserRoleNameAsync(int userId);
    }
}
