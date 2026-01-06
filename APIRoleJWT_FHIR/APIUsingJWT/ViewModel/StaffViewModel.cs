using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.ViewModel
{
    public class StaffViewModel
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
        public bool IsApproved { get; set; } = false;
        public ICollection<AdvertisementViewModel>? Advertisements { get; set; }

        public static StaffViewModel? FromModel(StaffModel? staffModel)
        {
            if (staffModel != null)
            {
                List<AdvertisementViewModel>? adViewList = staffModel.Advertisements?.Select(a => AdvertisementViewModel.FromModel(a)).ToList();

                return new StaffViewModel
                {
                    StaffId = staffModel.StaffId,
                    FullName = staffModel.FullName,
                    UserName = staffModel.UserName,
                    Email = staffModel.Email,
                    PhoneNumber = staffModel.PhoneNumber,
                    Address = staffModel.Address,
                    Gender = staffModel.Gender,
                    Age = staffModel.Age,
                    JoinedDate = staffModel.JoinedDate,
                    Salary = staffModel.Salary,
                    IsApproved = staffModel.IsApproved,
                    Advertisements = adViewList
                };
            }
            return null;

        }
    }
}
