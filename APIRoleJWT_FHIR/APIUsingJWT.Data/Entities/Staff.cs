using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Entities
{
    public class Staff
    {
        public Staff ()
        {
            Advertisements = new List<Advertisement> ();
        }
        [Key]
        public int StaffId { get; set; }
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [StringLength(40)]
        public string UserName { get; set; } = string.Empty;
        [Column(TypeName ="nvarchar(max)")]
        public string PasswordHash { get; set; } = string.Empty;
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;
        [Range(1000000000, 9999999999)]
        public long PhoneNumber { get; set; } = 0;
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;
        [Range(18,60)]
        public int Age { get; set; } = 18;
        [Column(TypeName ="date")]
        public DateTime JoinedDate { get; set; }
        [Range(10000, int.MaxValue)]
        public int? Salary { get; set; } = 10000;
        public bool IsApproved { get; set; } = true;
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public ICollection<Advertisement>? Advertisements { get; set; }

        public Staff FromModel(StaffModel staffModel)
        {
            if (staffModel != null)
            {
                this.StaffId = staffModel.StaffId;
                this.FullName = staffModel.FullName;
                this.UserName = staffModel.UserName;
                this.PasswordHash = staffModel.PasswordHash;
                this.Email = staffModel.Email;
                this.PhoneNumber = staffModel.PhoneNumber;
                this.Address = staffModel.Address;
                this.Gender = staffModel.Gender;
                this.Age = staffModel.Age;
                this.JoinedDate = staffModel.JoinedDate;
                this.Salary = staffModel.Salary;
                this.IsApproved = staffModel.IsApproved;
                this.RefreshTokenHash = staffModel.RefreshTokenHash;
                this.RefreshTokenExpiry = staffModel.RefreshTokenExpiry;
            }
            return this;
        }

        public StaffModel ToModel()
        {
            return new StaffModel
            {
                StaffId = this.StaffId,
                FullName = this.FullName,
                UserName = this.UserName,
                PasswordHash = this.PasswordHash,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Address = this.Address,
                Gender = this.Gender,
                Age = this.Age,
                JoinedDate = this.JoinedDate,
                Salary = this.Salary,
                IsApproved = this.IsApproved,
                RefreshTokenHash = this.RefreshTokenHash,
                RefreshTokenExpiry = this.RefreshTokenExpiry
            };
        }
    }
}
