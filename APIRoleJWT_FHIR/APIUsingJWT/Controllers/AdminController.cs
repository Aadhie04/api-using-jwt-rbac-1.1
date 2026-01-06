using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Services;
using APIUsingJWT.Domain.DTOs;
using APIUsingJWT.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Runtime.InteropServices;

namespace APIUsingJWT.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : BaseApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IStaffService _staffService;
        private readonly IArticleService _articleService;

        public AdminController(IConfiguration configuration, IUserService userService, IStaffService staffService, IArticleService articleService)
        {
            this._configuration = configuration;
            this._userService = userService;
            this._staffService = staffService;
            this._articleService = articleService;
        }

        [HttpGet("GetPendingUsers")]
        public async Task<ActionResult<List<UserViewModel>>> GetPendingUsersAsync()
        {
            List<UserModel> users = await _userService.GetPendingUsersAsync();
            List<UserViewModel?> userViewModels = new List<UserViewModel?>();
            foreach (var user in users)
            {
                userViewModels.Add(UserViewModel.FromModel(user));
            }
            return Ok(userViewModels);
        }

        [HttpGet("GetApprovedUsers")]
        public async Task<ActionResult<List<UserViewModel>>> GetApprovedUsersAsync()
        {
            List<UserModel> users = await _userService.GetApprovedUsersAsync();
            List<UserViewModel?> userViewModels = new List<UserViewModel?>();
            //if(users.Count == 0)
            //{
            //    return Ok("No approved users"); // dont do this cuz client expect List<UserResponse>
            //Suddenly it receives a string...JSON deserialization fails
            //}
            foreach (var user in users)
            {
                userViewModels.Add(UserViewModel.FromModel(user));
            }
            return Ok(userViewModels); // always return a list even if it is empty
        }

        [HttpPut("ApproveUser/{id}")]
        public async Task<ActionResult<bool>> ApproveUserAsync(int id)
        {
            if (id > 0)
            {
                //taking credentials from JWT
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;

                bool result = await _userService.ApproveUserAsync(id, performedBy, role);
                if (result)
                {  
                    
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Approval failed.");
                }

            }
            return NotFound("User not found.");

        }

        [HttpPut("RejectUser/{id}")]
        public async Task<ActionResult<bool>> RejectUserAsync(int id)
        {
            if (id > 0)
            {
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _userService.RejectUserAsync(id, performedBy, role);
                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Rejection failed.");
                }

            }
            return NotFound("User not found.");

        }

        [HttpGet("GetPendingArticles")]
        public async Task<ActionResult<List<ArticleViewModel>>> GetPendingArticlesAsync()
        {
            List<ArticleModel> articleModels = await _articleService.GetPendingArticlesAsync();
            List<ArticleViewModel?> articleViewModels = new List<ArticleViewModel?>();
            foreach (var article in articleModels)
            {
                articleViewModels.Add(ArticleViewModel.FromModel(article));
            }
            return Ok(articleViewModels);
        }
        [HttpGet("GetApprovedArticles")]
        public async Task<ActionResult<List<ArticleViewModel>>> GetApprovedArticlesAsync()
        {
            List<ArticleModel> articles = await _articleService.GetApprovedArticlesAsync();
            List<ArticleViewModel?> articleViewModels = new List<ArticleViewModel?>();
            foreach (var article in articles)
            {
                articleViewModels.Add(ArticleViewModel.FromModel(article));
            }
            return Ok(articleViewModels);
        }

        [HttpPut("ApproveArticle/{id}")]
        public async Task<ActionResult<bool>> ApproveArticleAsync(int id)
        {
            if (id > 0)
            {
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _articleService.ApproveArticleAsync(id, performedBy, role);
                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Approval failed.");
                }
            }
            return NotFound("Article not found.");
        }

        [HttpPut("RejectArticle/{id}")]
        public async Task<ActionResult<bool>> RejectArticleAsync(int id)
        {
            if (id > 0)
            {
                var performedBy = User.Identity?.Name!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _articleService.RejectArticleAsync(id, performedBy, role);
                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Rejection failed.");
                }

            }
            return NotFound("Article not found.");
        }
    }
}
