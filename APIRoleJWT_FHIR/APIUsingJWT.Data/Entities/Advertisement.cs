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
    public class Advertisement
    {
        [Key]
        public int AdvertisementId { get; set; }
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        [StringLength(100)]
        public string? ImagePath { get; set; }
        [Column(TypeName ="date")]
        public DateTime? CreatedDate { get; set; }
        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        public Advertisement FromModel(AdvertisementModel adModel)
        {
            if (adModel != null)
            {
                this.AdvertisementId = adModel.AdvertisementId;
                this.Title = adModel.Title;
                this.Description = adModel.Description;
                this.ImagePath = adModel.ImagePath;
                this.CreatedDate = adModel.CreatedDate;
                this.StaffId = adModel.StaffId;
            }
            return this;
        }

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
