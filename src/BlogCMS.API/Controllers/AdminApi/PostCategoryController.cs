using AutoMapper;
using BlogCMS.Core.Domain.Content;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.PostCategories;
using BlogCMS.Core.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BlogCMS.Core.SeedWorks.Constants.Permissions;

namespace BlogCMS.API.Controllers.AdminApi
{
    [Route("api/admin/post-category")]
    [ApiController]
    public class PostCategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostCategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("paging")]
        [Authorize(PostCategories.View)]
        public async Task<ActionResult<PagedResult<PostCategoryDto>>> GetPostCategoriesPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _unitOfWork.PostCategories.GetAllPaging(keyword, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(PostCategories.View)]
        public async Task<ActionResult<PostCategoryDto>> GetPostCategoryById(Guid id)
        {
            var category = await _unitOfWork.PostCategories.GetByIdAsync(id);
            if (category == null) return NotFound();
            var categoryDto = _mapper.Map<PostCategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpGet]
        [Authorize(PostCategories.View)]
        public async Task<ActionResult<List<PostCategoryDto>>> GetAllPostCategories()
        {
            var query = await _unitOfWork.PostCategories.GetAllAsync();
            var result = _mapper.Map<List<PostCategoryDto>>(query);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(PostCategories.Create)]
        public async Task<IActionResult> CreatePostCategory([FromBody] CreateUpdatePostCategoryRequest request)
        {
            var category = _mapper.Map<CreateUpdatePostCategoryRequest, PostCategory>(request);

            _unitOfWork.PostCategories.Add(category);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpPut("{id}")]
        [Authorize(PostCategories.Edit)]
        public async Task<IActionResult> EditPostCategory(Guid id, [FromBody] CreateUpdatePostCategoryRequest request)
        {
            var category = await _unitOfWork.PostCategories.GetByIdAsync(id);
            if (category == null) return NotFound();

            _mapper.Map(request, category);
            return Ok();
        }

        [HttpDelete]
        [Authorize(PostCategories.Delete)]
        public async Task<IActionResult> DeletePostCategory([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var category = await _unitOfWork.PostCategories.GetByIdAsync(id);
                if(category == null) return NotFound();
                _unitOfWork.PostCategories.Remove(category);
            }
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }
    }
}
