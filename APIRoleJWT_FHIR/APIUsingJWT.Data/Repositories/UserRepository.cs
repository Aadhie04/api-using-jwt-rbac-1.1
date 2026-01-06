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
    public class UserRepository : IUserRepository
    {
        private readonly ApiDbContext _context;

        public UserRepository(ApiDbContext context)
        {
            this._context = context;
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Articles).Select(u => new UserModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                IsApproved = u.IsApproved,
                Articles = u.Articles.Select(a => a.ToModel()).ToList()
            }).ToListAsync();
        }

        public async Task<UserModel> GetUserByIdAsync(int userid)
        {
            UserModel? user = await _context.Users.Include(u => u.Articles).Where(u => u.UserId == userid)
            .Select(u => new UserModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                IsApproved = u.IsApproved,
                Articles = u.Articles.Select(a => a.ToModel()).ToList()
            }).FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
                return null;
        }
        public async Task<UserModel> CreateUserAsync(UserModel userModel)
        {
            userModel.IsApproved = false;
            User user = new User();
            user.FromModel(userModel);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.ToModel();
        }
        public async Task<bool> UpdateUserAsync(UserModel userModel)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userModel.UserId);
            if (user != null)
            {
                userModel.IsApproved = true;
                user.FromModel(userModel);
                _context.Users.Update(user);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }

        public async Task<bool> DeleteUserAsync(int userid)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userid);
            if (user != null)
            {
                _context.Users.Remove(user);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }

        public async Task<List<UserModel>> GetPendingUsersAsync()
        {
            return await _context.Users.Where(u => !u.IsApproved).Select(u => new UserModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                IsApproved = u.IsApproved
            }).ToListAsync();
        }

        public async Task<List<UserModel>> GetApprovedUsersAsync()
        {
            return await _context.Users.Where(u => u.IsApproved).Select(u => new UserModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                IsApproved = u.IsApproved
            }).ToListAsync();
        }

        public async Task<bool> ApproveUserAsync(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsApproved = true;
                _context.Users.Update(user);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }

        public async Task<bool> RejectUserAsync(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;

        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            UserModel? user = await _context.Users.Where(u => u.UserName == username)
            .Select(u => new UserModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                IsApproved = u.IsApproved
            }).FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
                return null;
        }
        public async Task<UserModel?> GetByRefreshTokenHashAsync(string refreshTokenHash)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.RefreshTokenHash == refreshTokenHash)
                .Select(u => new UserModel
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    PasswordHash = u.PasswordHash,
                    Email = u.Email,
                    IsApproved = u.IsApproved,
                    RefreshTokenHash = u.RefreshTokenHash,
                    RefreshTokenExpiry = u.RefreshTokenExpiry
                }).FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> UpdateUserRefreshTokenAsync(int userId, string? refreshTokenHash, DateTime? expiry)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                user.RefreshTokenHash = refreshTokenHash;
                user.RefreshTokenExpiry = expiry;
                _context.Users.Update(user);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false; 
        }

        public async Task<List<string>> GetUserPermissionAsync(int userId)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == userId).SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.PermissionName).Distinct().ToListAsync();
        }
        public async Task AssignUserRoleAsync(int userId, int roleId)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetRoleIdByName(string roleName)
        {
            return await _context.Roles.Where(r => r.RoleName == roleName).Select(r => r.RoleId).FirstAsync();
        }
        public async Task<string>GetUserRoleNameAsync(int userId)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.RoleName).FirstOrDefaultAsync();
        }

    }
}
