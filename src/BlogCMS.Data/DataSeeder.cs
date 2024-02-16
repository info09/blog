using BlogCMS.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace BlogCMS.Data
{
    public class DataSeeder
    {
        public async Task SeedAsync(BlogCMSContext context)
        {
            var passwordHasher = new PasswordHasher<AppUser>();

            var rootAdminRoleId = Guid.NewGuid();
            if (!context.Roles.Any())
            {
                await context.Roles.AddAsync(new AppRole()
                {
                    Id = rootAdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    DisplayName = "Quản trị viên"
                });
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var userId = Guid.NewGuid();
                var user = new AppUser()
                {
                    Id = userId,
                    FirstName = "Huy",
                    LastName = "Tran",
                    Email = "huytq3103@gmail.com",
                    NormalizedEmail = "HUYTQ3103@GMAIL.COM",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                    DateCreated = DateTime.Now,
                };
                user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123$");
                await context.Users.AddAsync(user);
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>()
                {
                    RoleId = rootAdminRoleId,
                    UserId = userId
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
