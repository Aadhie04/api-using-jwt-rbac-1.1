using APIUsingJWT.Data.DbEntities;
using APIUsingJWT.Data.Entities;
using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Repositories
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly ApiDbContext _context;

        public AdvertisementRepository(ApiDbContext context)
        {
            this._context = context;
        }

        public async Task<List<AdvertisementModel>> GetAllAdvertisementsAsync()
        {
            return await _context.Advertisements.Include(ad => ad.Staff).Select(ad => new AdvertisementModel
            {
                AdvertisementId = ad.AdvertisementId,
                Title = ad.Title,
                Description = ad.Description,
                ImagePath = ad.ImagePath,
                CreatedDate = ad.CreatedDate,
                StaffId = ad.StaffId,
                Staff = ad.Staff.ToModel()
            }).ToListAsync();
        }

        public async Task<AdvertisementModel> GetAdvertisementByIdAsync(int advertisementid)
        {
            var advertisement = await _context.Advertisements.Include(ad => ad.Staff).Where(ad => ad.AdvertisementId == advertisementid)
            .Select(ad => new AdvertisementModel
            {
                AdvertisementId = ad.AdvertisementId,
                Title = ad.Title,
                Description = ad.Description,
                ImagePath = ad.ImagePath,
                CreatedDate = DateTime.Now,
                StaffId = ad.StaffId,
                Staff = ad.Staff.ToModel()
            }).FirstOrDefaultAsync();
            if (advertisement != null)
            {
                return advertisement;
            }
            else
                return null;
        }

        public async Task<AdvertisementModel> CreateAdvertisementAsync(AdvertisementModel advertisementModel)
        {
            Staff? staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == advertisementModel.StaffId);
            if (staff != null)
            {
                Advertisement advertisement = new Advertisement();
                advertisement.FromModel(advertisementModel);
                _context.Advertisements.Add(advertisement);
                await _context.SaveChangesAsync();
                return advertisement.ToModel();
            }
            return advertisementModel;
        }

        public async Task<bool> UpdateAdvertisementAsync(AdvertisementModel advertisementModel)
        {
            Advertisement? advertisement = await _context.Advertisements.FirstOrDefaultAsync(ad => ad.AdvertisementId == advertisementModel.AdvertisementId);
            if (advertisement != null)
            {
                Staff? staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == advertisementModel.StaffId);
                if (staff != null)
                {
                    advertisement.FromModel(advertisementModel);
                    _context.Advertisements.Update(advertisement);
                    bool result = await _context.SaveChangesAsync() > 0;
                    return result;
                }
                else
                    return false;
            }
            return false;
        }

        public async Task<bool> DeleteAdvertisementAsync(int advertisementid)
        {
            Advertisement? advertisement = await _context.Advertisements.FirstOrDefaultAsync(a => a.AdvertisementId == advertisementid);
            if (advertisement != null)
            {
                _context.Advertisements.Remove(advertisement);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }
    }
}
