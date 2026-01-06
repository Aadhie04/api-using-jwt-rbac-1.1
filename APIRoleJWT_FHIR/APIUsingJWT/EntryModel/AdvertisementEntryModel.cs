using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.EntryModel
{
    public class AdvertisementEntryModel
    {
        public int AdvertisementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int StaffId { get; set; }

        public AdvertisementModel ToModel()
        {
            return new AdvertisementModel
            {
                AdvertisementId = this.AdvertisementId,
                Title = this.Title,
                Description = this.Description,
                ImagePath = this.ImagePath,
                CreatedDate = this.CreatedDate,
                StaffId = this.StaffId
            };
        }
    }
}
