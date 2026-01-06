using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.EntryModel
{
    public class ArticleEntryModel
    {
        public int ArticleId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int UserId { get; set; }

        public ArticleModel ToModel()
        {
            return new ArticleModel
            {
                ArticleId = this.ArticleId,
                Title = this.Title,
                Content = this.Content,
                Topic = this.Topic,
                ImagePath = this.ImagePath,
                CreatedDate = this.CreatedDate,
                UserId = this.UserId
            };
        }
    }
}
