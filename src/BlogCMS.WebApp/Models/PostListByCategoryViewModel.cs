using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.PostCategories;
using BlogCMS.Core.Models.Content.Posts;

namespace BlogCMS.WebApp.Models
{
    public class PostListByCategoryViewModel
    {
        public PostCategoryDto Category { get; set; }
        public PagedResult<PostInListDto> Posts { get; set; }
    }
}
