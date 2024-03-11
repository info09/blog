using BlogCMS.Core.Models.Content.PostCategories;
using BlogCMS.Core.Models.Content.Posts;
using BlogCMS.Core.Models.Content.Tags;

namespace BlogCMS.WebApp.Models.Contents
{
    public class PostDetailViewModel
    {
        public PostDto Post { get; set; }
        public PostCategoryDto Category { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
