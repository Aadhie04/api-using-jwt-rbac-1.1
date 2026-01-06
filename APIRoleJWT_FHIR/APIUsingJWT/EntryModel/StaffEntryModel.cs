using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.EntryModel
{
    public class StaffEntryModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = "Male";
        public int Age { get; set; } = 18;
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public int? Salary { get; set; } = 10000;

        public StaffModel ToModel()
        {
            return new StaffModel
            {
                StaffId = this.StaffId,
                FullName = this.FullName,
                UserName = this.UserName,
                PasswordHash = this.Password,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Address = this.Address,
                Gender = this.Gender,
                Age = this.Age,
                JoinedDate = this.JoinedDate,
                Salary = this.Salary
            };
        }
    }
}
