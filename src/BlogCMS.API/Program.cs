using BlogCMS.API;
using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Models.Content;
using BlogCMS.Core.SeedWorks;
using BlogCMS.Data;
using BlogCMS.Data.Repositories;
using BlogCMS.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddDbContext<BlogCMSContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<BlogCMSContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Business services and repositoríes
var services = typeof(PostRepository).Assembly.GetTypes()
                    .Where(i => i.GetInterfaces().Any(i => i.Name == typeof(IRepository<,>).Name) && 
                                                            !i.IsAbstract && i.IsClass && !i.IsGenericType);

foreach (var item in services)
{
    var allInterfaces = item.GetInterfaces();
    var directInterface = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault();
    if (directInterface != null)
    {
        builder.Services.Add(new ServiceDescriptor(directInterface, item, ServiceLifetime.Scoped));
    }
}

builder.Services.AddAutoMapper(typeof(PostInListDto));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
    });
    c.SwaggerDoc("AdminAPI", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "API for Administrators",
        Description = "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
    });
    c.ParameterFilter<SwaggerNullableParameterFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("AdminAPI/swagger.json", "Admin API");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MigrationDatabase();

app.Run();
