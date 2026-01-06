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
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<ArticleService> _logger;
        private readonly IAuditLogService _auditLogService;

        public ArticleService(IArticleRepository articleRepository, ILogger<ArticleService> logger,
            IAuditLogService auditLogService)
        {
            this._articleRepository = articleRepository;
            this._logger = logger;
            this._auditLogService = auditLogService;
        }

        public async Task<List<ArticleModel>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllArticlesAsync();
        }

        public async Task<ArticleModel> GetArticleByIdAsync(int articleid)
        {
            return await _articleRepository.GetArticleByIdAsync(articleid);
        }

        public Task<ArticleModel> CreateArticleAsync(ArticleModel article)
        {
            return _articleRepository.CreateArticleAsync(article);
        }

        public async Task<bool> UpdateArticleAsync(ArticleModel article)
        {
            return await _articleRepository.UpdateArticleAsync(article);
        }

        public async Task<bool> DeleteArticleAsync(int articleid, string performedBy, string role)
        {
            _logger.LogInformation("Requesting to delete an article Id {articleId}", articleid);
            var result = await _articleRepository.DeleteArticleAsync(articleid);
            if(result)
            {
                await _auditLogService.LogAsync(
                action: "DELETE",
                entityId: articleid,
                entity: "Article",
                performedBy: performedBy,
                role: role);
                _logger.LogInformation("Successfully deleted article {articleId}", articleid);
            }
            else
            {
                _logger.LogWarning("Failed to delete article {articleId}", articleid);
            }
            return result;
        }
        public async Task<List<ArticleModel>> GetPendingArticlesAsync()
        {
            return await _articleRepository.GetPendingArticlesAsync();
        }
        public async Task<List<ArticleModel>> GetApprovedArticlesAsync()
        {
            return await _articleRepository.GetApprovedArticlesAsync();
        }
        public async Task<bool> ApproveArticleAsync(int id, string performedBy, string role)
        {
            _logger.LogInformation("Admin attempting to approve user, ID: {id}", id);
            var result = await _articleRepository.ApproveArticleAsync(id);
            if (result)
            {
                await _auditLogService.LogAsync(
                action: "APPROVE",
                entityId: id,
                entity: "Article",
                performedBy: performedBy,
                role: role);
                _logger.LogInformation("Article approval successfull!, ID: {id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to approve article, ID: {id}", id);
            }
            return result;
        }
        public async Task<bool> RejectArticleAsync(int id, string performedBy, string role)
        {
            _logger.LogInformation("Admin attempting to reject article, ID: {id}", id);
            var result = await _articleRepository.RejectArticleAsync(id);
            if (result)
            {
                await _auditLogService.LogAsync(
                action: "REJECT",
                entityId: id,
                entity: "Article",
                performedBy: performedBy,
                role: role);
                _logger.LogInformation("Article rejected successfully!, ID: {id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to reject Article, ID: {id}", id);
            }
            return result;
        }
    }
}
