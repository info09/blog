using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.SeedWorks;

namespace BlogCMS.Core.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<List<Post>> GetPopularPostsAsync(int count);
    }
}
