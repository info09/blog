using AutoMapper;
using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Repositories;
using BlogCMS.Core.SeedWorks;
using BlogCMS.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogCMSContext _context;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> userManager;

        public UnitOfWork(BlogCMSContext context, IMapper mapper, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _context = context;
            Posts = new PostRepository(context, mapper, userManager);
            Users = new UserRepository(context, roleManager);
            PostCategories = new PostCategoryRepository(context, mapper);
        }

        public IPostRepository Posts { get; private set; }
        public IUserRepository Users { get; private set; }

        public IPostCategoryRepository PostCategories { get; private set; }

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
