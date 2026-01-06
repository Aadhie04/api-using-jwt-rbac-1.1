using APIUsingJWT.Data.DbEntities;
using APIUsingJWT.Data.Entities;
using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Repositories
{
    public class ArticleRepository :IArticleRepository
    {
        private readonly ApiDbContext _context;

        public ArticleRepository(ApiDbContext context)
        {
            this._context = context;
        }

        public async Task<List<ArticleModel>> GetAllArticlesAsync()
        {
            return await _context.Articles.Include(a => a.User).Select(a => new ArticleModel
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Topic = a.Topic,
                Content = a.Content,
                ImagePath = a.ImagePath,
                CreatedDate = a.CreatedDate,
                IsApproved = a.IsApproved,
                UserId = a.UserId,
                User = a.User.ToModel()
            }).ToListAsync();
        }

        public async Task<ArticleModel> GetArticleByIdAsync(int articleid)
        {
            ArticleModel? article = await _context.Articles.Include(a => a.User).Where(a => a.ArticleId == articleid).Select(a => new ArticleModel
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Topic = a.Topic,
                Content = a.Content,
                ImagePath = a.ImagePath,
                CreatedDate = a.CreatedDate,
                IsApproved = a.IsApproved,
                UserId = a.UserId,
                //User = a.User.ToModel()
                User = a.User != null ? a.User.ToModel() : null!
            }).FirstOrDefaultAsync();
            if (article != null)
            {
                return article;
            }
            else
                return null;


        }

        public async Task<ArticleModel> CreateArticleAsync(ArticleModel articleModel)
        {
            User? User = await _context.Users.FirstOrDefaultAsync(u => u.UserId == articleModel.UserId);
            if (User != null)
            {
                articleModel.IsApproved = false;
                Article article = new Article();
                article.FromModel(articleModel);
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return article.ToModel();
            }
            return articleModel;
        }

        public async Task<bool> UpdateArticleAsync(ArticleModel articleModel)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == articleModel.UserId);
            if (user != null)
            {
                Article? article = await _context.Articles.FirstOrDefaultAsync(u => u.ArticleId == articleModel.ArticleId);
                if (article != null)
                {
                    article.FromModel(articleModel);
                    _context.Articles.Update(article);
                    bool result = await _context.SaveChangesAsync() > 0;
                    return result;
                }
                else
                    return false;
            }
            return false;

        }

        public async Task<bool> DeleteArticleAsync(int articleid)
        {
            Article? article = await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId == articleid);
            if (article != null)
            {
                _context.Articles.Remove(article);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }
        public async Task<List<ArticleModel>> GetPendingArticlesAsync()
        {
            return await _context.Articles.Where(a => !a.IsApproved).Select(a => new ArticleModel
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                Topic = a.Topic,
                CreatedDate = a.CreatedDate,
                IsApproved = a.IsApproved,
                UserId = a.UserId
            }).ToListAsync();
        }

        public async Task<List<ArticleModel>> GetApprovedArticlesAsync()
        {
            return await _context.Articles.Where(a => a.IsApproved).Select(a => new ArticleModel
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                Topic = a.Topic,
                CreatedDate = a.CreatedDate,
                IsApproved = a.IsApproved,
                UserId = a.UserId
            }).ToListAsync();
        }

        public async Task<bool> ApproveArticleAsync(int id)
        {
            Article? article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                article.IsApproved = true;
                _context.Articles.Update(article);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;
        }

        public async Task<bool> RejectArticleAsync(int id)
        {
            Article? article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                bool result = await _context.SaveChangesAsync() > 0;
                return result;
            }
            else
                return false;

        }
    }
}
