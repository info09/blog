using AutoMapper;
using BlogCMS.Core.Domain.Content;

namespace BlogCMS.Core.Models.Content.PostCategories
{
    public class PostCategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { set; get; }
        public required string Slug { set; get; }
        public Guid? ParentId { set; get; }
        public bool IsActive { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime? DateModified { set; get; }
        public string? SeoDescription { set; get; }
        public int SortOrder { set; get; }
        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<PostCategory, PostCategoryDto>();
            }
        }
    }
}
