using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Services
{
    public interface IStaffService
    {
        Task<List<StaffModel>> GetAllStaffsAsync();
        Task<StaffModel> GetStaffByIdAsync(int id);
        Task<StaffModel> CreateStaffAsync(StaffModel staff, string performedBy, string role);
        Task<bool> UpdateStaffAsync(StaffModel staff);
        Task<bool> DeleteStaffAsync(int id, string performedBy, string role);
        Task<StaffModel> ValidateLoginAsync(string username, string password);
        Task<StaffModel?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateStaffRefreshTokenAsync(int staffId, string refreshTokenHash, DateTime expiry);
        Task<bool> RevokeStaffRefreshTokenAsync(int staffId);
        Task<List<string>> GetStaffPermissionsAsync(int staffId);
        Task<string> GetStaffRoleNameAsync(int staffId);
    }
}
