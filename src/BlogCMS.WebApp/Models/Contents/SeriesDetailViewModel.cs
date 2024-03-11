using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.Posts;
using BlogCMS.Core.Models.Content.Series;

namespace BlogCMS.WebApp.Models.Contents
{
    public class SeriesDetailViewModel
    {
        public SeriesDto Series { get; set; }

        public PagedResult<PostInListDto> Posts { get; set; }
    }
}
