using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BlogCMS.Data
{
    public class BlogCMSContextFactory : IDesignTimeDbContextFactory<BlogCMSContext>
    {
        public BlogCMSContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();
            var builder = new DbContextOptionsBuilder<BlogCMSContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new BlogCMSContext(builder.Options);
        }
    }
}
