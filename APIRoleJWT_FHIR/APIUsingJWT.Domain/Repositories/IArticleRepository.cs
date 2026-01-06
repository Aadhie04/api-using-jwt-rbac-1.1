using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Repositories
{
    public interface IArticleRepository
    {
        Task<List<ArticleModel>> GetAllArticlesAsync();
        Task<ArticleModel> GetArticleByIdAsync(int id);
        Task<ArticleModel> CreateArticleAsync(ArticleModel article);
        Task<bool> UpdateArticleAsync(ArticleModel article);
        Task<bool> DeleteArticleAsync(int id);
        Task<List<ArticleModel>> GetPendingArticlesAsync();
        Task<List<ArticleModel>> GetApprovedArticlesAsync();
        Task<bool> ApproveArticleAsync(int id);
        Task<bool> RejectArticleAsync(int id);
    }
}
