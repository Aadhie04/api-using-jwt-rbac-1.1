using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Models
{
    public class StaffModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = "Male";
        public int Age { get; set; } = 18;
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public int? Salary { get; set; } = 10000;
        public bool IsApproved { get; set; } = false;
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public ICollection<AdvertisementModel>? Advertisements { get; set; }
    }
}
