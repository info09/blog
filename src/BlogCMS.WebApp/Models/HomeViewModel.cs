using BlogCMS.Core.Models.Content.Posts;

namespace BlogCMS.WebApp.Models
{
    public class HomeViewModel
    {
        public List<PostInListDto> LatestPosts { get; set; }
    }
}
