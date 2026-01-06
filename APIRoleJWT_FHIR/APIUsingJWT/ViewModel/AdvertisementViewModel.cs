using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.ViewModel
{
    public class AdvertisementViewModel
    {
        public int AdvertisementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int StaffId { get; set; }
        public StaffViewModel? Staff { get; set; }
        public static AdvertisementViewModel? FromModel(AdvertisementModel? adModel)
        {
            if (adModel != null)
            {
                return new AdvertisementViewModel
                {
                    AdvertisementId = adModel.AdvertisementId,
                    Title = adModel.Title,
                    Description = adModel.Description,
                    ImagePath = adModel.ImagePath,
                    CreatedDate = adModel.CreatedDate,
                    StaffId = adModel.StaffId,
                    Staff = StaffViewModel.FromModel(adModel.Staff)
                };
            }
            return null;

        }
    }
}
