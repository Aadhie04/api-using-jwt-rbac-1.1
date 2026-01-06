using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using APIUsingJWT.Domain.Services;
using APIUsingJWT.Domain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Services.RepServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<object> _pwdhasher;
        private readonly ILogger<UserService> _logger;
        private readonly IAuditLogService _auditLogService;

        public UserService(IUserRepository userRepository, IPasswordHasher<object> pwdhasher, 
            ILogger<UserService> logger, IAuditLogService auditLogService) 
        {
            this._userRepository = userRepository;
            this._pwdhasher = pwdhasher;
            this._logger = logger;
            this._auditLogService = auditLogService;
        }
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
        public async Task<UserModel> GetUserByIdAsync(int userid)
        {
            return await _userRepository.GetUserByIdAsync(userid);
        }
        public Task<UserModel> CreateUserAsync(UserModel user)
        {
            var hashed = _pwdhasher.HashPassword(null, user.PasswordHash);
            user.PasswordHash = hashed;
            return _userRepository.CreateUserAsync(user);
        }
        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            var hashed = _pwdhasher.HashPassword(null, user.PasswordHash);
            user.PasswordHash = hashed;
            return await _userRepository.UpdateUserAsync(user);
        }
        public async Task<bool> DeleteUserAsync(int userid, string performedBy, string role)
        {
            var result = await _userRepository.DeleteUserAsync(userid);
            if(result)
            {
                await _auditLogService.LogAsync(
                action: "DELETE",
                entityId: userid,
                entity: "User",
                performedBy: performedBy,
                role: role);
            }
            return result;
            
        }
        public async Task<List<UserModel>> GetPendingUsersAsync()
        {
            return await _userRepository.GetPendingUsersAsync();
        }
        public async Task<List<UserModel>> GetApprovedUsersAsync()
        {
            return await _userRepository.GetApprovedUsersAsync();
        }
        public async Task<bool> ApproveUserAsync(int id, string performedBy, string role)
        {
            _logger.LogInformation("Admin attempting to approve user, ID: {id}", id);
            var result = await _userRepository.ApproveUserAsync(id);
            if(result)
            {
                await _auditLogService.LogAsync(
                action: "APPROVE",
                entityId: id,
                entity: "User",
                performedBy: performedBy,
                role: role);
                var roleId = await _userRepository.GetRoleIdByName("User"); //from Role table 
                await _userRepository.AssignUserRoleAsync(id, roleId); //tp UserRole Table
                _logger.LogInformation("User approval successfull!, ID: {id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to approve user, ID: {id}", id);
            }
            return result;
        }
        public async Task<bool> RejectUserAsync(int id, string performedBy, string role)
        {
            _logger.LogInformation("Admin attempting to reject user, ID: {id}", id);
            var result = await _userRepository.RejectUserAsync(id);
            if(result)
            {
                await _auditLogService.LogAsync(
                action: "REJECT",
                entityId: id,
                entity: "User",
                performedBy: performedBy,
                role: role);

                _logger.LogInformation("User rejected successfully!, ID: {id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to reject user, ID: {id}", id);
            }
            return result;
        }
        public async Task<UserModel?> GetByRefreshTokenAsync(string refreshToken)
        {
            var hashed = TokenHelper.HashToken(refreshToken);
            return await _userRepository.GetByRefreshTokenHashAsync(hashed);
        }
        public async Task<bool> UpdateUserRefreshTokenAsync(int userId, string refreshTokenHash, DateTime expiry)
        {
            _logger.LogInformation("Updating refresh token for user {UserId}, expiry {Expiry}",userId,expiry);
            var result = await _userRepository.UpdateUserRefreshTokenAsync(userId, refreshTokenHash, expiry);
            if(result)
            {
                _logger.LogInformation("Successfully updated refresh token for user {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Failed to update refresh token for user {UserId}", userId);
            }
            return result;
        }
        public async Task<UserModel> ValidateLoginAsync(string username, string password)
        {
            _logger.LogInformation("Login validation started for username: {username}", username);
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user != null)
            {
                var verify = _pwdhasher.VerifyHashedPassword(null, user.PasswordHash, password);
                if (verify == PasswordVerificationResult.Success || verify == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    if (verify == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        var newHash = _pwdhasher.HashPassword(null, password);
                        user.PasswordHash = newHash;
                        await _userRepository.UpdateUserAsync(user);
                    }
                    return user;
                }
                _logger.LogWarning("Login failed: Invalid password for {username}", username);
                return null;
            }
            else
                _logger.LogWarning("Login failed: {username} not found", username);
                return null;
        }
        public async Task<bool> RevokeUserRefreshTokenAsync(int userId)
        {
            _logger.LogInformation("Revoking token for user {UserId}", userId);
            var result = await _userRepository.UpdateUserRefreshTokenAsync(userId, null, null);
            if (result)
            {
                _logger.LogInformation("Refresh token revoked successfully for user {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Failed to revoke refresh token for user {UserId}", userId);
            }
            return result;
        }
        public async Task<List<string>> GetUserPermissionAsync(int userId)
        {
            _logger.LogInformation("Checking permissions for user {UserId}", userId);
            var result = await _userRepository.GetUserPermissionAsync(userId);
            if (result.Count == 0)
            {
                _logger.LogWarning("No permissions found for user {UserId}", userId);
            }
            else
            {
                _logger.LogInformation("permissions found for user {UserId}", userId);
            }
            return result;
        }
        public async Task<string> GetUserRoleNameAsync(int userId)
        {
            var result = await _userRepository.GetUserRoleNameAsync(userId);
            return result;
        }


    }
}
