using BlogCMS.Core.Domain.Royalty;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Royalty;
using BlogCMS.Core.SeedWorks;

namespace BlogCMS.Core.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction, Guid>
    {
        Task<PagedResult<TransactionDto>> GetAllPaging(string? userName, int fromMonth, int fromYear, int toMonth, int toYear, int pageIndex = 1, int pageSize = 10);
    }
}
