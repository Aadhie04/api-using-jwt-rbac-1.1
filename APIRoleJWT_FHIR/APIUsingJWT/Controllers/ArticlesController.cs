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
    public class ArticlesController : BaseApiController
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(IArticleService articleService, IUserService userService, ILogger<ArticlesController> logger)
        {
            this._articleService = articleService;
            this._userService = userService;
            this._logger = logger;
        }

        [Authorize(Policy = "Everyone")]
        [HttpGet("GetAllArticles")]
        public async Task<ActionResult<List<ArticleViewModel>>> GetAllArticlesAsync()
        {
            _logger.LogInformation("Fetching records of all articles.");
            List<ArticleModel> articleModels = await _articleService.GetAllArticlesAsync();
            if(articleModels == null || articleModels.Count == 0)
            {
                _logger.LogWarning("Articles not found.");
                return BadRequest("Articles not found.");
            }
            List<ArticleViewModel?> articles = new List<ArticleViewModel?>();
            foreach (ArticleModel articlemodel in articleModels)
            {
                articles.Add(ArticleViewModel.FromModel(articlemodel));
            }
            _logger.LogInformation("Fetched {count} Articles record", articleModels.Count);
            return Ok(articles);
        }

        [Authorize(Policy = "Everyone")]
        [HttpGet("GetArticleById/{id}")]
        public async Task<ActionResult<ArticleViewModel>> GetArticleByIdAsync(int id)
        {
            _logger.LogInformation("Fetching article with ID {id}", id);
            ArticleModel? articleModel = await _articleService.GetArticleByIdAsync(id);
            if (articleModel == null)
            {
                _logger.LogWarning("Article not found");
                return NotFound("Article not found.");
            }
            _logger.LogInformation("Fetched article, ID {ID}", id);
            return Ok(ArticleViewModel.FromModel(articleModel));
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPost("CreateArticle")]
        public async Task<ActionResult<ArticleViewModel>> CreateArticleAsync([FromBody] ArticleEntryModel articleEntryModel)
        {
            _logger.LogInformation("Attempting to create article, topic {articleTopic}", articleEntryModel.Topic);
            var user = await _userService.GetUserByIdAsync(articleEntryModel.UserId);
            if (user != null)
            {
                if (user.IsApproved)
                {
                    ArticleModel? articleModel = articleEntryModel.ToModel();
                    ArticleModel? result = await _articleService.CreateArticleAsync(articleModel);
                    if (result != null)
                    {
                        _logger.LogInformation("Article created successfully, Title {Title}", articleModel.Title);
                        return Ok(ArticleViewModel.FromModel(result));
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create article");
                        return BadRequest("Article not created.");
                    }
                }
                else
                {
                    _logger.LogWarning("Request from an unapproved user");
                    return Unauthorized("Your account is not approved yet. Please wait for admin approval.");
                }
            }
            else
                _logger.LogWarning("User not found");
                return NotFound("User not found.");

        }

        [Authorize(Policy = "UserOnly")]
        [HttpPut("UpdateArticleById/{id}")]
        public async Task<ActionResult<bool>> UpdateArticleAsync(int id, [FromBody] ArticleEntryModel articleEntryModel)
        {
            _logger.LogInformation("Article update request, Title: {Title}", articleEntryModel.Title);
            if (id != articleEntryModel.ArticleId)
            {
                _logger.LogWarning("Article ID mismatch, requestId {RequestId}, bodyId {BodyId}", id, articleEntryModel.ArticleId);
                return BadRequest("Invalid request.");
            }
            if (articleEntryModel.ArticleId > 0)
            {
                ArticleModel? articleModel = articleEntryModel.ToModel();
                bool result = await _articleService.UpdateArticleAsync(articleModel);
                if (result)
                {
                    _logger.LogInformation("Article updation successfull, ID: {ID}", articleModel.ArticleId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Article updation failed");
                    BadRequest("Update failed.");
                }
            }
            _logger.LogWarning("Article records not found.");
            return NotFound("Article not found.");
        }

        [Authorize(Policy = "StaffsandAdmin")]
        [HttpDelete("DeleteArticleById/{id}")]
        public async Task<ActionResult<bool>> DeleteArticleAsync(int id)
        {
            _logger.LogInformation("Attempting to delete article. ID: {Id}", id);
            if (id > 0)
            {
                //var performedBy = User.Identity?.Name!;
                var performedBy = User.FindFirst(ClaimTypes.Name)?.Value!;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;
                bool result = await _articleService.DeleteArticleAsync(id, performedBy,role);
                if (result)
                {
                    _logger.LogInformation("Article deleted successfully! ID: {Id}", id);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Delete operation failed for article, Id: {Id}", id);
                    BadRequest("Delete failed.");
                }
            }
            _logger.LogWarning("Article not found.");
            return NotFound("Article not found.");

        }
    }
}
