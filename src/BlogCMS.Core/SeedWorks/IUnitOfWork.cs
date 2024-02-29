using BlogCMS.Core.Repositories;

namespace BlogCMS.Core.SeedWorks
{
    public interface IUnitOfWork
    {
        IPostRepository Posts { get; }
        IUserRepository Users { get; }
        IPostCategoryRepository PostCategories { get; }
        ISeriesRepository Series { get; }
        Task<int> CompleteAsync();
    }
}
