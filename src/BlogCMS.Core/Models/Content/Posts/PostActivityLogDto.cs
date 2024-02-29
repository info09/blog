using AutoMapper;
using BlogCMS.Core.Domain.Content;

namespace BlogCMS.Core.Models.Content.Posts
{
    public class PostActivityLogDto
    {
        public PostStatus FromStatus { set; get; }

        public PostStatus ToStatus { set; get; }

        public DateTime DateCreated { get; set; }

        public string? Note { set; get; }

        public string UserName { get; set; }

        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<PostActivityLog, PostActivityLogDto>();
            }
        }
    }
}
