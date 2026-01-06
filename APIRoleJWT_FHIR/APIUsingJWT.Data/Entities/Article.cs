using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Entities
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        [StringLength(100)]
        public string Topic { get; set; } = string.Empty;
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;
        [StringLength(10000)]
        public string Content { get; set; } = string.Empty;
        [StringLength(300)]
        public string? ImagePath { get; set; }
        [Column(TypeName ="date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;
        public int UserId { get; set; }
        public User? User { get; set; }

        public Article FromModel(ArticleModel articleModel)
        {
            if (articleModel != null)
            {
                this.ArticleId = articleModel.ArticleId;
                this.Title = articleModel.Title;
                this.Content = articleModel.Content;
                this.Topic = articleModel.Topic;
                this.ImagePath = articleModel.ImagePath;
                this.IsApproved = articleModel.IsApproved;
                this.CreatedDate = articleModel.CreatedDate;
                this.UserId = articleModel.UserId;
            }
            return this;
        }

        public ArticleModel ToModel()
        {
            return new ArticleModel
            {
                ArticleId = this.ArticleId,
                Title = this.Title,
                Content = this.Content,
                Topic = this.Topic,
                ImagePath = this.ImagePath,
                IsApproved = this.IsApproved,
                CreatedDate = this.CreatedDate,
                UserId = this.UserId
            };
        }

    }
}
