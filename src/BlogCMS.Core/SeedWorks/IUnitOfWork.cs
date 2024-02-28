using BlogCMS.Core.Repositories;

namespace BlogCMS.Core.SeedWorks
{
    public interface IUnitOfWork
    {
        IPostRepository Posts { get; }
        IUserRepository Users { get; }
        IPostCategoryRepository PostCategories { get; }
        Task<int> CompleteAsync();
    }
}
