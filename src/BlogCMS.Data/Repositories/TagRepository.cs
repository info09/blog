using AutoMapper;
using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Models.Content.Tags;
using BlogCMS.Core.Repositories;
using BlogCMS.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Data.Repositories
{
    public class TagRepository : RepositoryBase<Tag, Guid>, ITagRepository
    {
        private readonly IMapper _mapper;
        public TagRepository(BlogCMSContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<TagDto?> GetBySlug(string slug)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(i => i.Slug == slug);
            return tag == null ? null : _mapper.Map<TagDto?>(tag);
        }
    }
}
