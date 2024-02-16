using AutoMapper;
using BlogCMS.Core.Repositories;
using BlogCMS.Core.SeedWorks;
using BlogCMS.Data.Repositories;

namespace BlogCMS.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogCMSContext _context;

        public UnitOfWork(BlogCMSContext context, IMapper mapper)
        {
            _context = context;
            Posts = new PostRepository(context, mapper);
        }

        public IPostRepository Posts { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
