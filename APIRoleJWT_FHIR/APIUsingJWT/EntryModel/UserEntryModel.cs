using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.EntryModel
{
    public class UserEntryModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public UserModel ToModel()
        {
            return new UserModel
            {
                UserId = this.UserId,
                UserName = this.UserName,
                PasswordHash = this.Password,
                Email = this.Email
            };
        }
    }
}
