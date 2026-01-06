using APIUsingJWT.Domain.Models;

namespace APIUsingJWT.ViewModel
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;

        public ICollection<ArticleViewModel>? Articles { get; set; }

        public static UserViewModel? FromModel(UserModel? userModel)
        {
            if (userModel != null)
            {
                List<ArticleViewModel>? articleViewList = userModel.Articles?.Select(a => ArticleViewModel.FromModel(a)).ToList();
                return new UserViewModel
                {
                    UserId = userModel.UserId,
                    UserName = userModel.UserName,
                    Email = userModel.Email,
                    IsApproved = userModel.IsApproved,
                    Articles = articleViewList
                };
            }
            return null;

        }
    }
}
