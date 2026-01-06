using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Models
{
    public class AdvertisementModel
    {
        public int AdvertisementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int StaffId { get; set; }
        public StaffModel Staff { get; set; } = null!;


    }
}
