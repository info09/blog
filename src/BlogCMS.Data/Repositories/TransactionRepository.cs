using AutoMapper;
using BlogCMS.Core.Domain.Royalty;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Royalty;
using BlogCMS.Core.Repositories;
using BlogCMS.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Data.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction, Guid>, ITransactionRepository
    {
        private readonly IMapper _mapper;
        public TransactionRepository(BlogCMSContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult<TransactionDto>> GetAllPaging(string? keyword, int fromMonth, int fromYear, int toMonth, int toYear, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Transactions.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(i => i.ToUserName.Contains(keyword.ToLower().Trim()));

            if (fromMonth > 0 && fromYear > 0)
                query = query.Where(i => i.DateCreated.Month >= fromMonth && i.DateCreated.Year >= fromYear);

            if (toMonth > 0 && toYear > 0)
                query = query.Where(i => i.DateCreated.Month <= toMonth && i.DateCreated.Year >= toYear);

            var totalRow = await query.CountAsync();

            query = query.OrderByDescending(i => i.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PagedResult<TransactionDto>
            {
                RowCount = totalRow,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = await _mapper.ProjectTo<TransactionDto>(query).ToListAsync()
            };
        }
    }
}
