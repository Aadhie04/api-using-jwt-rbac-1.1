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
    public class StaffsController : BaseApiController
    {
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffsController> _logger;

        public StaffsController(IStaffService staffService, ILogger<StaffsController> logger)
        {
            this._staffService = staffService;
            this._logger = logger;
        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpGet("GetAllStaffs")]
        public async Task<ActionResult<List<StaffViewModel>>> GetAllStaffAsync()
        {
            _logger.LogInformation("Fetching list of staff...");
            List<StaffModel> staffList = await _staffService.GetAllStaffsAsync();
            if(staffList == null || staffList.Count == 0)
            {
                _logger.LogWarning("No staff record found");
                return BadRequest("No staff found");
            }
            List<StaffViewModel?> staffs = new List<StaffViewModel?>();
            foreach (StaffModel staffModel in staffList)
            {
                staffs.Add(StaffViewModel.FromModel(staffModel));
            }
            _logger.LogInformation("Fetched {count} staff records",staffList.Count);
            return Ok(staffs);
        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpGet("GetStaffById/{id}")]
        public async Task<ActionResult<StaffViewModel>> GetStaffByIdAsync(int id)
        {
            _logger.LogInformation("Fetching staff with ID {id}", id);
            StaffModel? staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                _logger.LogWarning("No staff found");
                return NotFound("Staff not found.");
            }
            _logger.LogInformation("Fetched the details of staff. Username: {username}", staff.UserName);
            return Ok(StaffViewModel.FromModel(staff));
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("CreateStaff")]
        public async Task<ActionResult<StaffViewModel>> CreateStaffAsync([FromBody] StaffEntryModel staffEntryModel)
        {
            _logger.LogInformation("Admin attempting to create staff with username {UserName}", staffEntryModel.UserName);
            var performedBy = User.Identity?.Name!;
            var role = User.FindFirst(ClaimTypes.Role)?.Value!;
            StaffModel? staffModel = staffEntryModel.ToModel();
            StaffModel? result = await _staffService.CreateStaffAsync(staffModel, performedBy, role);
            if (result != null)
            {
                _logger.LogInformation("Staff created successfully with username: {StaffName}", staffEntryModel.UserName);
                return Ok(StaffViewModel.FromModel(result));
            }
            else
            {
                _logger.LogWarning("Invalid staff creation request");
                return BadRequest("Staff not created.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("UpdateStaffById/{id}")]
        public async Task<ActionResult<bool>> UpdateStaffAsync(int id, [FromBody] StaffEntryModel staffEntryModel)
        {
            _logger.LogInformation("Staff update request, username: {UserName}", staffEntryModel.UserName);
            if (id != staffEntryModel.StaffId)
            {
                _logger.LogWarning("Staff ID mismatch, route {RouteId}, body {BodyId}", id, staffEntryModel.StaffId);
                return BadRequest("Invalid request.");
            }
            if (staffEntryModel.StaffId > 0)
            {
                _logger.LogInformation("Updating staff, username: {UserName}", staffEntryModel.UserName);
                StaffModel? staffModel = staffEntryModel.ToModel();
                bool result = await _staffService.UpdateStaffAsync(staffModel);
                if (result)
                {
                    _logger.LogInformation("Staff Update successful! username: {UserName}", staffEntryModel.UserName);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Staff update failed for username {Username}", staffEntryModel.UserName);
                    return BadRequest("Update failed.");
                }
            }
            return NotFound("Staff not found.");

        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("DeleteStaffById/{id}")]
        public async Task<ActionResult<bool>> DeleteStaffAsync(int id)
        {
            _logger.LogInformation("Admin attempting to delete staff with ID {ID}", id);
            if (id > 0)
            {
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _staffService.DeleteStaffAsync(id, performedBy, role);
                if (result)
                {
                    _logger.LogInformation("Staff deleted, ID: {ID}", id);
                    return result;
                }
                else
                {
                    _logger.LogWarning("Delete operation failed for staff, Id: {Id}", id);
                    return BadRequest("Delete failed.");
                }
            }
            _logger.LogWarning("User not found");
            return NotFound("Staff not found.");
        }
    }
}
