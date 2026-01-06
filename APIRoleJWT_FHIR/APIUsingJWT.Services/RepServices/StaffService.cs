using APIUsingJWT.Data.Entities;
using APIUsingJWT.Data.Repositories;
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
        public class StaffService : IStaffService
        {
            private readonly IStaffRepository _staffRepository;
            private readonly IPasswordHasher<object> _pwdhasher;
            private readonly ILogger<StaffService> _logger;
            private readonly IAuditLogService _auditLogService;

            public StaffService(IStaffRepository staffRepository, IPasswordHasher<object> pwdhasher, 
                ILogger<StaffService> logger, IAuditLogService auditLogService)
            {
                this._staffRepository = staffRepository;
                this._pwdhasher = pwdhasher;
                this._logger = logger;
                this._auditLogService = auditLogService;
            }

            public async Task<List<StaffModel>> GetAllStaffsAsync()
            {
                return await _staffRepository.GetAllStaffsAsync();
            }

            public async Task<StaffModel> GetStaffByIdAsync(int staffid)
            {
                return await  _staffRepository.GetStaffByIdAsync(staffid);
            }

            public async Task<StaffModel> CreateStaffAsync(StaffModel staff, string performedBy, string role)
            {
                var hashed = _pwdhasher.HashPassword(null, staff.PasswordHash);
                staff.PasswordHash = hashed;
                var result = await _staffRepository.CreateStaffAsync(staff);
                await _auditLogService.LogAsync(
                   action: "Create",
                   entityId: result.StaffId,
                   entity: "Staff",
                   performedBy: performedBy,
                   role: role
                );
                var roleId = await _staffRepository.GetRoleIdByName("Staff");
                await _staffRepository.AssignUserRoleAsync(result.StaffId, roleId);
                return result;
            }
            public async Task<bool> UpdateStaffAsync(StaffModel staff)
            {
                var hashed = _pwdhasher.HashPassword(null, staff.PasswordHash);
                staff.PasswordHash = hashed;
                return await _staffRepository.UpdateStaffAsync(staff);
            }

            public async Task<bool> DeleteStaffAsync(int staffid, string performedBy, string role)
            {
                var result = await _staffRepository.DeleteStaffAsync(staffid);
                if (result)
                {
                    await _auditLogService.LogAsync(
                    action: "DELETE",
                    entityId: staffid,
                    entity: "Staff",
                    performedBy: performedBy,
                    role: role);
                }
                return result;
            }
            public async Task<StaffModel?> GetByRefreshTokenAsync(string refreshToken)
            {
                var hashed = TokenHelper.HashToken(refreshToken);
                return await _staffRepository.GetByRefreshTokenHashAsync(hashed);
            }
            public async Task<bool> UpdateStaffRefreshTokenAsync(int staffId,  string refreshTokenHash, DateTime expiry)
            {
                _logger.LogInformation("Updating refresh token for staff {StaffId}, expiry {Expiry}", staffId, expiry);
                var result =  await _staffRepository.UpdateStaffRefreshTokenAsync(staffId, refreshTokenHash, expiry);
                if (result)
                {
                    _logger.LogInformation("Successfully updated refresh token for user {StaffId}", staffId);
                }
                else
                {
                    _logger.LogWarning("Failed to update refresh token for user {StaffId}", staffId);
                }
                return result;
            }

            public async Task<StaffModel> ValidateLoginAsync(string username, string password)
            {
                _logger.LogInformation("Login validation started for username: {username}", username);
                var staff = await _staffRepository.GetStaffByUsernameAsync(username);
                    if (staff != null)
                    {
                        var verify = _pwdhasher.VerifyHashedPassword(null, staff.PasswordHash, password);
                        if (verify == PasswordVerificationResult.Success || verify == PasswordVerificationResult.SuccessRehashNeeded)
                        {
                            if(verify == PasswordVerificationResult.SuccessRehashNeeded)
                            {
                                var newHash = _pwdhasher.HashPassword(null, staff.PasswordHash);
                                staff.PasswordHash = newHash;
                                await _staffRepository.UpdateStaffAsync(staff);
                            }
                            return staff;
                        }
                        _logger.LogWarning("Login failed: Invalid password for {username}", username);
                        return null;
                    }
                    else
                        _logger.LogWarning("Login failed: {username} not found", username);
                        return null;
            }
            public async Task<bool> RevokeStaffRefreshTokenAsync(int staffId)
            {
                _logger.LogInformation("Revoking token for staff {StaffId}", staffId);
                var result = await _staffRepository.UpdateStaffRefreshTokenAsync(staffId, null, null);
                if (result)
                {
                    _logger.LogInformation("Refresh token revoked successfully for staff {StaffId}", staffId);
                }
                else
                {
                    _logger.LogWarning("Failed to revoke refresh token for staff {StaffId}", staffId);
                }
                return result;
            }
            public async Task<List<string>> GetStaffPermissionsAsync(int staffId)
            {
                _logger.LogInformation("Checking permissions for staff {StaffId}", staffId);
                var result = await _staffRepository.GetStaffPermissionsAsync(staffId);
                if (result.Count == 0)
                {
                    _logger.LogWarning("No permissions found for staff {StaffId}", staffId);
                }
                else
                {
                    _logger.LogInformation("permissions found for staff {StaffId}", staffId);
                }
                return result;
            }
            public async Task<string> GetStaffRoleNameAsync(int staffId)
            {
                var result = await _staffRepository.GetStaffRoleNameAsync(staffId);
                return result;
            }

        }
}
