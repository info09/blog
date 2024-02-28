using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.SeedWorks;

namespace BlogCMS.Core.Repositories
{
    public interface IUserRepository : IRepository<AppUser, Guid>
    {
        Task RemoveRolesFromUser(AppUser user, string[] roles);
    }
}
