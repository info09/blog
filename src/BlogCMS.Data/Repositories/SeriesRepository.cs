using AutoMapper;
using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.Posts;
using BlogCMS.Core.Models.Content.Series;
using BlogCMS.Core.Repositories;
using BlogCMS.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Data.Repositories
{
    public class SeriesRepository : RepositoryBase<Series, Guid>, ISeriesRepository
    {
        private readonly IMapper _mapper;
        public SeriesRepository(BlogCMSContext context, IMapper mapper) : base(context)
        {
            this._mapper = mapper;
        }

        public async Task AddPostToSeries(Guid seriesId, Guid postId, int sortOrder)
        {
            var postInSeries = await _context.PostInSeries.AnyAsync(i => i.PostId == postId && i.SeriesId == seriesId);
            if (!postInSeries)
            {
                await _context.PostInSeries.AddAsync(new PostInSeries { PostId = postId, SeriesId = seriesId, DisplayOrder = sortOrder });
            }
        }

        public async Task<PagedResult<SeriesInListDto>> GetAllPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Series.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(i => i.Name.ToLower().Contains(keyword.ToLower()));

            var totalRow = await query.CountAsync();
            query = query.OrderByDescending(i => i.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PagedResult<SeriesInListDto>
            {
                RowCount = totalRow,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = await _mapper.ProjectTo<SeriesInListDto>(query).ToListAsync()
            };
        }

        public async Task<List<PostInListDto>> GetAllPostsInSeries(Guid seriesId)
        {
            var query = (from pis in _context.PostInSeries
                         join p in _context.Posts on pis.PostId equals p.Id
                         where pis.SeriesId == seriesId
                         select p);
            var result = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync();
            return result;
        }

        public async Task<PagedResult<PostInListDto>> GetAllPostsInSeries(string slug, int pageIndex = 1, int pageSize = 10)
        {
            var query = from pis in _context.PostInSeries
                        join p in _context.Posts
                        on pis.PostId equals p.Id
                        join s in _context.Series
                        on pis.SeriesId equals s.Id
                        where s.Slug == slug
                        select p;
            var totalRow = await query.CountAsync();

            query = query.OrderByDescending(i => i.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                RowCount = totalRow,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<SeriesDto> GetBySlug(string slug)
        {
            var series = await _context.Series.FirstOrDefaultAsync(i => i.Slug == slug);
            return _mapper.Map<SeriesDto>(series);
        }

        public async Task<bool> HasPost(Guid seriesId)
        {
            return await _context.PostInSeries.AnyAsync(i => i.SeriesId == seriesId);
        }

        public async Task<bool> IsPostInSeries(Guid seriesId, Guid postId)
        {
            return await _context.PostInSeries.AnyAsync(i => i.PostId == postId && i.SeriesId == seriesId);
        }

        public async Task RemovePostToSeries(Guid seriesId, Guid postId)
        {
            var postInSeries = await _context.PostInSeries.FirstOrDefaultAsync(i => i.SeriesId == seriesId && i.PostId == postId);
            if (postInSeries != null)
            {
                _context.PostInSeries.Remove(postInSeries);
            }
        }
    }
}
