using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.Posts;

namespace BlogCMS.WebApp.Models.Contents
{
    public class ListPostByUserViewModel
    {
        public string Keyword { get; set; }
        public int TotalPosts { get; set; }
        public int TotalDraftPosts { get; set; }
        public int TotalWaitingApprovalPosts { get; set; }
        public int TotalPublishedPosts { get; set; }
        public int TotalUnpaidPosts { get; set; }
        public double TotalUnpaidAmount { get; set; }
        public double TotalPaidAmount { get; set; }
        public PagedResult<PostInListDto> Posts { get; set; }
    }
}
