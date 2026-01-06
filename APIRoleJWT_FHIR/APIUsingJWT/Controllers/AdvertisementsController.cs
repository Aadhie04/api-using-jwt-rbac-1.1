using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Services;
using APIUsingJWT.EntryModel;
using APIUsingJWT.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIUsingJWT.Controllers
{
    public class AdvertisementsController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly ILogger<AdvertisementsController> _logger;
        private readonly IStaffService _staffService;

        public AdvertisementsController(IAdvertisementService advertisementService, ILogger<AdvertisementsController> logger,
            IStaffService staffService)
        {
            this._advertisementService = advertisementService;
            this._logger = logger;
            this._staffService = staffService;
        }

        [Authorize]
        [HttpGet("GetAllAdvertisements")]
        public async Task<ActionResult<List<AdvertisementViewModel>>> GetAllAdvertisementsAsync()
        {
            _logger.LogInformation("Fetching records of all advertisements.");
            List<AdvertisementModel> adModels = await _advertisementService.GetAllAdvertisementsAsync();
            if(adModels == null || adModels.Count == 0)
            {
                _logger.LogWarning("advertisements not found.");
                return BadRequest("advertisements not found.");
            }
            List<AdvertisementViewModel?> ads = new List<AdvertisementViewModel?>();
            foreach (AdvertisementModel ad in adModels)
            {
                ads.Add(AdvertisementViewModel.FromModel(ad));
            }
            _logger.LogInformation("Fetched {count} advertisements record", adModels.Count);
            return Ok(ads);
        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpGet("GetAdvertisementById/{id}")]
        public async Task<ActionResult<AdvertisementViewModel>> GetAdvertisementByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Advertisement with ID {id}", id);
            AdvertisementModel? ad = await _advertisementService.GetAdvertisementByIdAsync(id);
            if (ad == null)
            {
                _logger.LogWarning("Advertisement not found");
                return NotFound("Advertisement not found.");

            }
            _logger.LogInformation("Fetched Advertisement, ID {ID}", id);
            return Ok(AdvertisementViewModel.FromModel(ad));

        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost("CreateAdvertisement")]
        public async Task<ActionResult<AdvertisementViewModel>> CreateAdvertisementAsync([FromBody] AdvertisementEntryModel advertisementEntryModel)
        {
            _logger.LogInformation("Attempting to create Advertisement, title {title}", advertisementEntryModel.Title);
            var staff = await _staffService.GetStaffByIdAsync(advertisementEntryModel.StaffId);
            if (staff != null)
            {
                var adModel = advertisementEntryModel.ToModel();
                var result = await _advertisementService.CreateAdvertisementAsync(adModel);
                if (result != null)
                {
                    _logger.LogInformation("Advertisement created successfully, Title {Title}", advertisementEntryModel.Title);
                    return Ok(AdvertisementViewModel.FromModel(result));
                }
                else
                {
                    _logger.LogWarning("Failed to create Advertisement, Title {Title}", advertisementEntryModel.Title);
                    return BadRequest("Advertisement not created.");
                }
            }
            else
                _logger.LogWarning("Staff not found");
                return NotFound("staff not found");

        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpPut("UpdateAdvertisementById/{id}")]
        public async Task<ActionResult<bool>> UpdateAdvertisementAsync(int id, [FromBody] AdvertisementEntryModel advertisementEntryModel)
        {
            _logger.LogInformation("Advertisement update request, Title: {Title}", advertisementEntryModel.Title);
            if (id != advertisementEntryModel.AdvertisementId)
            {
                _logger.LogWarning("Advertisement ID mismatch, requestId {RequestId}, bodyId {BodyId}", id, advertisementEntryModel.AdvertisementId);
                return BadRequest("Invalid request.");
            }
            if (advertisementEntryModel.AdvertisementId > 0)
            {
                AdvertisementModel? adModel = advertisementEntryModel.ToModel();
                bool result = await _advertisementService.UpdateAdvertisementAsync(adModel);
                if (result)
                {
                    _logger.LogInformation("Advertisement updation successfull, ID: {ID}", advertisementEntryModel.AdvertisementId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Advertisement updation failed");
                    return BadRequest("Update failed.");
                }
            }
            _logger.LogWarning("Advertisement rocords not found");
            return NotFound("Advertisemnet not found.");

        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpDelete("DeleteAdvertisementById/{id}")]
        public async Task<ActionResult<bool>> DeleteAdvertisementAsync(int id)
        {
            _logger.LogInformation("Attempting to delete article. ID: {Id}", id);
            if (id > 0)
            {
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _advertisementService.DeleteAdvertisementAsync(id, performedBy, role);
                if (result)
                {
                    _logger.LogInformation("Advertisement deleted successfully! ID: {Id}", id);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Delete operation failed for Advertisement, Id: {Id}", id);
                    return BadRequest("Delete failed.");
                }
            }
            _logger.LogWarning("Advertisement not found.");
            return NotFound("Advertisement not found.");

        }
    }
}
