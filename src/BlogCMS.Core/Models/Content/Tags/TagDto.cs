using AutoMapper;
using BlogCMS.Core.Domain.Content;

namespace BlogCMS.Core.Models.Content.Tags
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public required string Name { get; set; }

        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<Tag, TagDto>();
            }
        }
    }
}
