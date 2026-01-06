using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Repositories
{
    public interface IStaffRepository
    {
        Task<List<StaffModel>> GetAllStaffsAsync();
        Task<StaffModel> GetStaffByIdAsync(int id);
        Task<StaffModel> CreateStaffAsync(StaffModel staff);
        Task<bool> UpdateStaffAsync(StaffModel staff);
        Task<bool> DeleteStaffAsync(int id);
        Task<StaffModel> GetStaffByUsernameAsync(string username);
        Task<StaffModel?> GetByRefreshTokenHashAsync(string refreshTokenHash);
        Task<bool> UpdateStaffRefreshTokenAsync(int staffId, string? refreshTokenHash, DateTime? expiry);
        Task<List<string>> GetStaffPermissionsAsync(int staffId);
        Task AssignUserRoleAsync(int staffId, int roleId);
        Task<int> GetRoleIdByName(string roleName);
        Task<string> GetStaffRoleNameAsync(int staffId);

    }
}
