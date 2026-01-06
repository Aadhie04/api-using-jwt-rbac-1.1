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
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            this._userService = userService;
            this._logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<UserViewModel>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching list of staff...");
            List<UserModel> users = await _userService.GetAllUsersAsync();
            if(users.Count == 0 || users == null)
            {
                _logger.LogWarning("No staff record found");
                return BadRequest("No staff found");
            }
            List<UserViewModel?> userViewModels = new List<UserViewModel?>();
            foreach (var user in users)
            {
                userViewModels.Add(UserViewModel.FromModel(user));
            }
            _logger.LogInformation("Fetched {count} staff record", users.Count);
            return Ok(userViewModels);
        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<UserViewModel>> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {id}", id);
            UserModel? userModel = await _userService.GetUserByIdAsync(id);
            if (userModel == null)
            {
                _logger.LogWarning("No user found");
                return NotFound("User not found.");
            }
            _logger.LogInformation("Fetched user, username: {username}", userModel.UserName);
            return Ok(UserViewModel.FromModel(userModel));
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserViewModel>> CreateAsync([FromBody] UserEntryModel userEntryModel)
        {
            _logger.LogInformation("Attempting to create user with username {UserName}", userEntryModel.UserName);
            UserModel userModel = userEntryModel.ToModel();
            UserModel result = await _userService.CreateUserAsync(userModel);
            if (result != null)
            {
                _logger.LogInformation("User created successfully, username: {username}", userModel.UserName);
                return Ok(UserViewModel.FromModel(result));
            }
            else
            {
                _logger.LogWarning("User creation failed");
                return BadRequest("No user created.");
            }
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPut("UpdateUserById/{id}")]
        public async Task<ActionResult<bool>> UpdateUserAsync(int id, [FromBody] UserEntryModel userEntryModel)
        {
            _logger.LogInformation("User update request, username: {UserName}", userEntryModel.UserName);
            if (id != userEntryModel.UserId)
            {
                _logger.LogWarning("User ID mismatch, requestId {RequestId}, bodyId {BodyId}", id, userEntryModel.UserId);
                return BadRequest("Invalid request.");
            }
            if (userEntryModel.UserId > 0)
            {
                _logger.LogInformation("User ID matched, requestId {RequestId}, bodyId {BodyId}", id, userEntryModel.UserId);
                UserModel? userModel = userEntryModel.ToModel();
                bool result = await _userService.UpdateUserAsync(userModel);
                if (result)
                {
                    _logger.LogInformation("User updation successfull, username: {Username}", userModel.UserName);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("User updation failed");
                    return BadRequest("Update failed.");
                }
            }
            return NotFound("User not found.");
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("DeleteUserById/{id}")]
        public async Task<ActionResult<bool>> DeleteUserAsync(int id)
        {
            _logger.LogInformation("Admin attempting to delete user. ID: {Id}",id);
            if (id > 0)
            {
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _userService.DeleteUserAsync(id, performedBy, role);
                if (result)
                {
                    _logger.LogInformation("User deleted successfully! ID: {ID}", id);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Delete operation failed for User, Id: {Id}", id);
                    return BadRequest("Delete failed.");
                }

            }
            _logger.LogWarning("User not found");
            return NotFound("User not found.");
        }
    }
}
