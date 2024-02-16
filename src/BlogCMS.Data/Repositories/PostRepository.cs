using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Repositories;
using BlogCMS.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
    {
        public PostRepository(BlogCMSContext context) : base(context)
        {
        }

        public Task<List<Post>> GetPopularPostsAsync(int count)
        {
            return _context.Posts.OrderByDescending(i => i.ViewCount).Take(count).ToListAsync();
        }
    }
}
