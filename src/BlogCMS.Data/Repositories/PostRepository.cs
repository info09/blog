using AutoMapper;
using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.Posts;
using BlogCMS.Core.Models.Content.Series;
using BlogCMS.Core.Repositories;
using BlogCMS.Core.SeedWorks.Constants;
using BlogCMS.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public PostRepository(BlogCMSContext context, IMapper mapper, UserManager<AppUser> userManager) : base(context)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task Approve(Guid id, Guid currentUserId)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Không tồn tại bài viết");

            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
            {
                throw new Exception("Không tồn tại user");
            }
            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.Published,
                UserId = currentUserId,
                UserName = user.UserName!,
                PostId = post.Id,
                Note = $"{user?.UserName} đã duyệt bài"
            });
            post.Status = PostStatus.Published;
            _context.Posts.Update(post);
        }

        public async Task<List<PostActivityLogDto>> GetActivityLogs(Guid id)
        {
            var query = _context.PostActivityLogs.Where(x => x.PostId == id).OrderByDescending(x => x.DateCreated);
            return await _mapper.ProjectTo<PostActivityLogDto>(query).ToListAsync();
        }

        public async Task<PagedResult<PostInListDto>> GetAllPaging(string? keyword, Guid currentUserId, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(currentUserId.ToString()) ?? throw new Exception("Không tồn tại user");

            var roles = await _userManager.GetRolesAsync(user);
            var canApprove = false;
            if (roles.Contains(Roles.Admin))
            {
                canApprove = true;
            }
            else
            {
                canApprove = await _context.RoleClaims.AnyAsync(i => roles.Contains(i.RoleId.ToString()) && i.ClaimValue == Permissions.Posts.Approve);
            }

            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(i => i.Name.ToLower().Contains(keyword.ToLower()));

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryId == categoryId.Value);

            if (!canApprove)
                query = query.Where(i => i.AuthorUserId == currentUserId);

            var totalRow = await query.CountAsync();
            query = query.OrderByDescending(i => i.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                PageSize = pageSize,
                RowCount = totalRow,
            };
        }

        public async Task<List<SeriesInListDto>> GetAllSeries(Guid postId)
        {
            var query = (from pis in _context.PostInSeries
                         join s in _context.Series on pis.SeriesId equals s.Id
                         where pis.PostId == postId
                         select s);
            return await _mapper.ProjectTo<SeriesInListDto>(query).ToListAsync();
        }

        public async Task<List<PostInListDto>> GetLatestPublishPost(int top)
        {
            var query = _context.Posts.Where(i => i.Status == PostStatus.Published).OrderByDescending(i => i.DateCreated).Take(top);

            var data = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync();

            return data;
        }

        public async Task<List<Post>> GetListUnpaidPublishPosts(Guid userId)
        {
            return await _context.Posts.Where(i => i.AuthorUserId == userId && i.IsPaid == false && i.Status == PostStatus.Published).ToListAsync();
        }

        public Task<List<Post>> GetPopularPostsAsync(int count)
        {
            return _context.Posts.OrderByDescending(i => i.ViewCount).Take(count).ToListAsync();
        }

        public async Task<PagedResult<PostInListDto>> GetPostByCategoryPaging(string categorySlug, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(categorySlug))
                query = query.Where(i => i.CategorySlug == categorySlug);

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

        public async Task<PagedResult<PostInListDto>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(i => i.Name.Trim().ToLower().Contains(keyword.Trim().ToLower()));

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryId == categoryId.Value);

            var totalRow = await query.CountAsync();

            query = query.OrderByDescending(i => i.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };
        }

        public async Task<string> GetReturnReason(Guid id)
        {
            var activity = await _context.PostActivityLogs.Where(i => i.PostId == id && i.ToStatus == PostStatus.Rejected).OrderByDescending(i => i.DateCreated).FirstOrDefaultAsync();
            return activity?.Note!;
        }

        public async Task<bool> HasPublishInLast(Guid id)
        {
            var hasPublished = await _context.PostActivityLogs.CountAsync(i => i.PostId == id && i.ToStatus == PostStatus.Published);
            return hasPublished > 0;
        }

        public Task<bool> IsSlugAlreadyExisted(string slug, Guid? currentId = null)
        {
            if (currentId.HasValue)
            {
                return _context.Posts.AnyAsync(i => i.Slug == slug && i.Id != currentId.Value);
            }
            return _context.Posts.AnyAsync(i => i.Slug == slug);
        }

        public async Task ReturnBack(Guid id, Guid currentUserId, string note)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Không tồn tại bài viết");

            var user = await _userManager.FindByIdAsync(currentUserId.ToString()) ?? throw new Exception("Không tồn tại user");

            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.Rejected,
                UserId = currentUserId,
                UserName = user.UserName,
                PostId = post.Id,
                Note = note
            });
            post.Status = PostStatus.Rejected;
            _context.Posts.Update(post);
        }

        public async Task SendToApprove(Guid id, Guid currentUserId)
        {
            var post = await _context.Posts.FindAsync(id) ?? throw new Exception("Không tồn tại bài viết");

            var user = await _context.Users.FindAsync(currentUserId) ?? throw new Exception("Không tồn tại user");

            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.WaitingForApproval,
                UserId = currentUserId,
                UserName = user.UserName,
                PostId = post.Id,
                Note = $"{user?.UserName} đã gửi bài chờ duyệt"
            });
            post.Status = PostStatus.WaitingForApproval;
            _context.Posts.Update(post);
        }
    }
}
