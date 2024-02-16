using BlogCMS.Core.SeedWorks;

namespace BlogCMS.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogCMSContext _context;

        public UnitOfWork(BlogCMSContext context)
        {
            _context = context;
        }

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
