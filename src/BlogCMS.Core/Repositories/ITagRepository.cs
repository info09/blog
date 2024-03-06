using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Models.Content.Tags;
using BlogCMS.Core.SeedWorks;

namespace BlogCMS.Core.Repositories
{
    public interface ITagRepository : IRepository<Tag, Guid>
    {
        Task<TagDto?> GetBySlug(string slug);
    }
}
