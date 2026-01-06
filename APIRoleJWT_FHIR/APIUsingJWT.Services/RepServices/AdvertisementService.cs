using APIUsingJWT.Data.Entities;
using APIUsingJWT.Data.Repositories;
using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using APIUsingJWT.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Services.RepServices
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly ILogger<AdvertisementService> _logger;
        private readonly IAuditLogService _auditLogService;

        public AdvertisementService(IAdvertisementRepository advertisementRepository, 
            ILogger<AdvertisementService> logger, IAuditLogService auditLogService)
        {
            this._advertisementRepository = advertisementRepository;
            this._logger = logger;
            this._auditLogService = auditLogService;
        }

        public Task<List<AdvertisementModel>> GetAllAdvertisementsAsync()
        {
            return _advertisementRepository.GetAllAdvertisementsAsync();
        }
        public Task<AdvertisementModel> GetAdvertisementByIdAsync(int id)
        {
            return _advertisementRepository.GetAdvertisementByIdAsync(id);
        }

        public Task<AdvertisementModel> CreateAdvertisementAsync(AdvertisementModel advertisement)
        {
            return _advertisementRepository.CreateAdvertisementAsync(advertisement);
        }

        public Task<bool> UpdateAdvertisementAsync(AdvertisementModel advertisement)
        {
            return _advertisementRepository.UpdateAdvertisementAsync(advertisement);
        }

        public async Task<bool> DeleteAdvertisementAsync(int id, string performedBy, string role)
        {
            _logger.LogInformation("Requesting to delete an article Id {articleId}", id);
            var result = await _advertisementRepository.DeleteAdvertisementAsync(id);
            if (result)
            {
                await _auditLogService.LogAsync(
                    action: "DELETE",
                    entityId: id,
                    entity: "Advertisement",
                    performedBy: performedBy,
                    role: role);
                _logger.LogInformation("Successfully deleted article {articleId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete article {articleId}", id);
            }
            return result;
        }
    }
}
