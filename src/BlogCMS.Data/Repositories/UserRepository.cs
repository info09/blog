using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Repositories;
using BlogCMS.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Data.Repositories
{
    public class UserRepository : RepositoryBase<AppUser, Guid>, IUserRepository
    {
        private readonly RoleManager<AppRole> _roleManager;
        public UserRepository(BlogCMSContext context, RoleManager<AppRole> roleManager) : base(context)
        {
            _roleManager = roleManager;
        }

        public async Task RemoveRolesFromUser(AppUser user, string[] roles)
        {
            var roleIds = _roleManager.Roles.Where(x => roles.Contains(x.Name)).Select(x => x.Id).ToList();
            List<IdentityUserRole<Guid>> userRoles = new List<IdentityUserRole<Guid>>();
            foreach (var roleId in roleIds)
            {
                userRoles.Add(new IdentityUserRole<Guid> { RoleId = roleId, UserId = user.Id });
            }
            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
        }
    }
}
