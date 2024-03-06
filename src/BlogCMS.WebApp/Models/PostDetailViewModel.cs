using BlogCMS.Core.Models.Content.PostCategories;
using BlogCMS.Core.Models.Content.Posts;

namespace BlogCMS.WebApp.Models
{
    public class PostDetailViewModel
    {
        public PostDto Post { get; set; }
        public PostCategoryDto Category { get; set; }
    }
}
