using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Entities
{
    public class User
    {
        public User() 
        {
            Articles = new List<Article>();
        }
        [Key]
        public int UserId { get; set; }
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        [StringLength(40)]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public ICollection<Article> Articles { get; set; }

        public User FromModel(UserModel userModel)
        {
            if (userModel != null)
            {
                this.UserId = userModel.UserId;
                this.UserName = userModel.UserName;
                this.PasswordHash = userModel.PasswordHash;
                this.Email = userModel.Email;
                this.IsApproved = userModel.IsApproved;
                this.RefreshTokenHash = userModel.RefreshTokenHash;
                this.RefreshTokenExpiry = userModel.RefreshTokenExpiry;
            }
            return this;
        }

        public UserModel ToModel()
        {
            return new UserModel
            {
                UserId = this.UserId,
                UserName = this.UserName,
                PasswordHash = this.PasswordHash,
                Email = this.Email,
                IsApproved = this.IsApproved,
                RefreshTokenHash = this.RefreshTokenHash,
                RefreshTokenExpiry = this.RefreshTokenExpiry
            };
        }
    }
}
