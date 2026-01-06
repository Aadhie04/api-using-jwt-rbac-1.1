using APIUsingJWT.Domain.DTOs;
using APIUsingJWT.Domain.Services;
using APIUsingJWT.Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace APIUsingJWT.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IStaffService _staffService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IUserService userService, IStaffService staffService, 
            ITokenService tokenService, ILogger<AuthController> logger)
        {
            this._configuration = configuration;
            this._userService = userService;
            this._staffService = staffService;
            this._tokenService = tokenService;
            this._logger = logger;
        }

        [AllowAnonymous]
        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (login.Username == _configuration["AdminCredentials:Username"])
            {
                if(login.Password == _configuration["AdminCredentials:Password"])
                {
                    _logger.LogInformation("Login attempt for username {Username}", login.Username);
                    var access = _tokenService.GenerateAccessToken(login.Username, "Admin", 0, new List<string>());
                    var refresh = _tokenService.GenerateRefreshToken();
                    return Ok(new LoginResponse
                    {
                        Role = "Admin",
                        Message = "Admin login successful",
                        AccessToken = access,
                        RefreshToken = refresh
                    });
                }
                else
                    return Unauthorized(new { Message = "Admin Login: Invalid password!!!!!" });
            }

            var staff = await _staffService.ValidateLoginAsync(login.Username, login.Password);
            if (staff != null)
            {
                _logger.LogInformation("Staff login successful. StaffId {StaffId}, Username {UserName}", staff.StaffId, staff.UserName);

                var permissions = await _staffService.GetStaffPermissionsAsync(staff.StaffId);
                var roleFromDb = await _staffService.GetStaffRoleNameAsync(staff.StaffId);
                var access = _tokenService.GenerateAccessToken(staff.UserName, roleFromDb, staff.StaffId, permissions);
                var rawRefresh = _tokenService.GenerateRefreshToken();
                var hashedRefresh = TokenHelper.HashToken(rawRefresh);

                staff.RefreshTokenHash = hashedRefresh;
                staff.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
                await _staffService.UpdateStaffRefreshTokenAsync(staff.StaffId, hashedRefresh, staff.RefreshTokenExpiry.Value);
                return Ok(new LoginResponse 
                { 
                    Role = "Staff", 
                    Message = $"Staff {staff.FullName} logged in",
                    AccessToken = access,
                    RefreshToken = rawRefresh
                });

            }


            var user = await _userService.ValidateLoginAsync(login.Username, login.Password);
            if (user != null)
            {
                if (user.IsApproved)
                {
                    _logger.LogInformation("User login successful. UserId {UserId}, Username {UserName}", user.UserId, user.UserName);

                    var permissions = await _userService.GetUserPermissionAsync(user.UserId);
                    var roleFromDb = await _userService.GetUserRoleNameAsync(user.UserId);
                    var access = _tokenService.GenerateAccessToken(user.UserName, roleFromDb, user.UserId, permissions);
                    var rawRefresh = _tokenService.GenerateRefreshToken();
                    var hashedRefresh = TokenHelper.HashToken(rawRefresh);
                    user.RefreshTokenHash = hashedRefresh;
                    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
                    await _userService.UpdateUserRefreshTokenAsync(user.UserId, hashedRefresh, user.RefreshTokenExpiry.Value);

                    return Ok(new LoginResponse
                    { 
                        Role = "User", 
                        Message = $"User {user.UserName} logged in", 
                        AccessToken = access,
                        RefreshToken = rawRefresh
                    });
                }
                else
                {
                    _logger.LogWarning("Login blocked: User {UserName} is not approved", user.UserName);
                    return Unauthorized(new { Message = "Your account is pending admin approval" });
                }
            }
            _logger.LogInformation("Invalid login attempt for usrename {Usrename}", login.Username);
            return Unauthorized(new { Message = "Invalid username or password." });

        }

        [AllowAnonymous]
        [EnableRateLimiting("RefreshPolicy")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            _logger.LogInformation("Refresh token request received");

            if (string.IsNullOrEmpty(req.RefreshToken)) 
                return BadRequest();
            var hashed = TokenHelper.HashToken(req.RefreshToken);

            var user = await _userService.GetByRefreshTokenAsync(req.RefreshToken);
            if (user != null && user.RefreshTokenExpiry > DateTime.UtcNow)
            {
                _logger.LogInformation("Refresh token validated for user {Username}", user.UserName);
                var permissions = await _userService.GetUserPermissionAsync(user.UserId);
                var roleFromDb = await _userService.GetUserRoleNameAsync(user.UserId);
                var newAccess = _tokenService.GenerateAccessToken(user.UserName, roleFromDb, user.UserId, permissions);
                var newRawRefresh = _tokenService.GenerateRefreshToken();
                var newHashed = TokenHelper.HashToken(newRawRefresh);
                user.RefreshTokenHash = newHashed;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
                await _userService.UpdateUserRefreshTokenAsync(user.UserId, newHashed, user.RefreshTokenExpiry.Value);
                return Ok(new RefreshResponse
                { 
                    AccessToken = newAccess, 
                    RefreshToken = newRawRefresh 
                });
            }
            var staff = await _staffService.GetByRefreshTokenAsync(req.RefreshToken);
            if (staff != null && staff.RefreshTokenExpiry > DateTime.UtcNow)
            {
                _logger.LogInformation("Refresh token validated for staff {Username}", staff.UserName);
                var permissions = await _staffService.GetStaffPermissionsAsync(staff.StaffId);
                var roleFromDb = await _staffService.GetStaffRoleNameAsync(staff.StaffId);
                var newAccess = _tokenService.GenerateAccessToken(staff.UserName, roleFromDb, staff.StaffId, permissions);
                var newRawRefresh = _tokenService.GenerateRefreshToken();
                var newHashed = TokenHelper.HashToken(newRawRefresh);
                staff.RefreshTokenHash = newHashed;
                staff.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
                await _staffService.UpdateStaffRefreshTokenAsync(staff.StaffId, newHashed, staff.RefreshTokenExpiry.Value);
                return Ok(new RefreshResponse
                { 
                    AccessToken = newAccess, 
                    RefreshToken = newRawRefresh 
                });
            }
            _logger.LogWarning("Invalid or expired refresh token used");
            return Unauthorized("Invalid or expired refresh token");

        }

        [Authorize(Policy = "Everyone")]
        [HttpPost("revoke")]
        public async Task<IActionResult?> Revoke()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var idClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(idClaim)) 
                return BadRequest();

            int id = int.Parse(idClaim);
            if (role == "User")
            {
                await _userService.RevokeUserRefreshTokenAsync(id);
                return Ok();
            }
            else if (role == "Staff")
            {
                await _staffService.RevokeStaffRefreshTokenAsync(id);
                return Ok();
            }
            else if (role == "Admin")
            {
                return Ok();
            }
            return null;
        }

    }
}
