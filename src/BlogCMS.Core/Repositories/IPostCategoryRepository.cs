using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.PostCategories;
using BlogCMS.Core.SeedWorks;

namespace BlogCMS.Core.Repositories
{
    public interface IPostCategoryRepository : IRepository<PostCategory, Guid>
    {
        Task<PagedResult<PostCategoryDto>> GetAllPaging(string? keyword, int pageIndex = 1, int pageSize = 10);
        Task<bool> HasPost(Guid categoryId);
        Task<PostCategoryDto> GetBySlug(string slug);
    }
}
