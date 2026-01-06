using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.ViewModel
{
    public class ArticleViewModel
    {
        public int ArticleId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public UserViewModel? User { get; set; }

        public static ArticleViewModel? FromModel(ArticleModel? articleModel)
        {

            if (articleModel != null)
            {
                return new ArticleViewModel
                {
                    ArticleId = articleModel.ArticleId,
                    Title = articleModel.Title,
                    Content = articleModel.Content,
                    Topic = articleModel.Topic,
                    ImagePath = articleModel.ImagePath,
                    IsApproved = articleModel.IsApproved,
                    CreatedDate = articleModel.CreatedDate,
                    UserId = articleModel.UserId,
                    User = UserViewModel.FromModel(articleModel.User)
                };
            }
            return null;

        }
    }
}
