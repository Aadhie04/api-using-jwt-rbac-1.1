using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Repositories
{
    public interface IAdvertisementRepository
    {
        Task<List<AdvertisementModel>> GetAllAdvertisementsAsync();
        Task<AdvertisementModel> GetAdvertisementByIdAsync(int id);
        Task<AdvertisementModel> CreateAdvertisementAsync(AdvertisementModel advertisement);
        Task<bool> UpdateAdvertisementAsync(AdvertisementModel advertisement);
        Task<bool> DeleteAdvertisementAsync(int id);
    }
}
