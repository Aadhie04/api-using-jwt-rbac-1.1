using APIUsingJWT.Data.DbEntities;
using APIUsingJWT.Data.Entities;
using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApiDbContext _context;

        public StaffRepository(ApiDbContext context)
        {
            this._context = context;
        }

        public async Task<List<StaffModel>> GetAllStaffsAsync()
        {
            return await _context.Staffs.Include(s => s.Advertisements).Select(s => new StaffModel
            {
                StaffId = s.StaffId,
                FullName = s.FullName,
                UserName = s.UserName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Address = s.Address,
                Gender = s.Gender,
                Age = s.Age,
                JoinedDate = s.JoinedDate,
                Salary = s.Salary,
                IsApproved = s.IsApproved,
                Advertisements = s.Advertisements.Select(a => a.ToModel()).ToList()
            }).ToListAsync();
        }

        public async Task<StaffModel> GetStaffByIdAsync(int staffid)
        {
            StaffModel? staff = await _context.Staffs.Include(s => s.Advertisements).Where(s => s.StaffId == staffid)
            .Select(s => new StaffModel
            {
                StaffId = s.StaffId,
                FullName = s.FullName,
                UserName = s.UserName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Address = s.Address,
                Gender = s.Gender,
                Age = s.Age,
                JoinedDate = s.JoinedDate,
                Salary = s.Salary,
                IsApproved = s.IsApproved,
                Advertisements = s.Advertisements.Select(a => a.ToModel()).ToList()
            }).FirstOrDefaultAsync();
            if (staff != null)
            {
                return staff;
            }
            else
                return null;
        }

        public async Task<StaffModel> CreateStaffAsync(StaffModel staffModel)
        {
            staffModel.IsApproved = true;
            Staff staff = new Staff();
            staff.FromModel(staffModel);
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            return staff.ToModel();
        }

        public async Task<bool> UpdateStaffAsync(StaffModel staffModel)
        {
            Staff? staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == staffModel.StaffId);
            if (staff != null)
            {
                staff.FromModel(staffModel);
                _context.Staffs.Update(staff);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }

        public async Task<bool> DeleteStaffAsync(int staffid)
        {
            Staff? staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == staffid);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }

        public async Task<StaffModel> GetStaffByUsernameAsync(string username)
        {
            StaffModel? staff = await _context.Staffs.Where(s => s.UserName == username)
            .Select(s => new StaffModel
            {
                StaffId = s.StaffId,
                FullName = s.FullName,
                UserName = s.UserName,
                Email = s.Email,
                PasswordHash = s.PasswordHash,
                IsApproved = s.IsApproved
            }).FirstOrDefaultAsync();
            if (staff != null)
            {
                return staff;
            }
            else
                return null;
        }
        public async Task<StaffModel?> GetByRefreshTokenHashAsync(string refreshTokenHash)
        {
            var staff = await _context.Staffs
                .AsNoTracking()
                .Where(s => s.RefreshTokenHash == refreshTokenHash)
                .Select(s => new StaffModel
                {
                    StaffId = s.StaffId,
                    FullName = s.FullName,
                    UserName = s.UserName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Address = s.Address,
                    Gender = s.Gender,
                    Age = s.Age,
                    JoinedDate = s.JoinedDate,
                    Salary = s.Salary,
                    RefreshTokenHash = s.RefreshTokenHash,
                    RefreshTokenExpiry = s.RefreshTokenExpiry
                }).FirstOrDefaultAsync();
            if (staff != null)
            {
                return staff;
            }
            else
                return null;
        }
        public async Task<bool> UpdateStaffRefreshTokenAsync(int staffId, string? refreshTokenHash, DateTime? expiry)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == staffId);
            if (staff != null)
            {
                staff.RefreshTokenHash = refreshTokenHash;
                staff.RefreshTokenExpiry = expiry;
                _context.Staffs.Update(staff);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else 
                return false;
        }

        public async Task<List<string>> GetStaffPermissionsAsync(int staffId)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == staffId).SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.PermissionName).Distinct().ToListAsync();
        }

        public async Task AssignUserRoleAsync(int staffId, int roleId)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = staffId,
                RoleId = roleId
            });
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetRoleIdByName(string roleName)
        {
            return await _context.Roles.Where(r => r.RoleName == roleName).Select(r => r.RoleId).FirstAsync();
        }
        public async Task<string> GetStaffRoleNameAsync(int staffId)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == staffId).Select(ur => ur.Role.RoleName).FirstOrDefaultAsync(); 
        }

    }
}
