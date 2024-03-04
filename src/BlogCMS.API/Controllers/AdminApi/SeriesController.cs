using AutoMapper;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.Content.Posts;
using BlogCMS.Core.Models.Content.Series;
using BlogCMS.Core.SeedWorks;
using BlogCMS.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BlogCMS.Core.SeedWorks.Constants.Permissions;

namespace BlogCMS.API.Controllers.AdminApi
{
    [Route("api/admin/series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SeriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Series.View)]
        public async Task<ActionResult<List<SeriesInListDto>>> GetAllSeries()
        {
            var result = await _unitOfWork.Series.GetAllAsync();
            var series = _mapper.Map<List<SeriesInListDto>>(result);

            return Ok(series);
        }

        [HttpGet("paging")]
        [Authorize(Series.View)]
        public async Task<ActionResult<PagedResult<SeriesInListDto>>> GetAllSeriesPaging(string? keyword, int pageIndex, int pageSize = 10)
        {
            var result = await _unitOfWork.Series.GetAllPaging(keyword, pageIndex, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Series.View)]
        public async Task<ActionResult<SeriesDto>> GetAllSeriesById(Guid id)
        {
            var result = await _unitOfWork.Series.GetByIdAsync(id);
            var data = _mapper.Map<SeriesDto>(result);
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Series.Create)]
        public async Task<IActionResult> CreateSeries([FromBody] CreateUpdateSeriesRequest request)
        {
            var post = _mapper.Map<CreateUpdateSeriesRequest, Core.Domain.Content.Series>(request);

            _unitOfWork.Series.Add(post);

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpPut]
        [Authorize(Series.Edit)]
        public async Task<IActionResult> UpdateSeries(Guid id, [FromBody] CreateUpdateSeriesRequest request)
        {
            var post = await _unitOfWork.Series.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            _mapper.Map(request, post);

            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpPost("post-series")]
        [Authorize(Series.Edit)]
        public async Task<IActionResult> AddPostSeries([FromBody] AddPostSeriesRequest request)
        {
            var isExisted = await _unitOfWork.Series.IsPostInSeries(request.SeriesId, request.PostId);
            if (isExisted)
            {
                return BadRequest($"Bài viết này đã nằm trong loạt bài.");
            }
            await _unitOfWork.Series.AddPostToSeries(request.SeriesId, request.PostId, request.SortOrder);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpDelete("post-series")]
        [Authorize(Series.Delete)]
        public async Task<IActionResult> DeletePostSeries([FromBody] AddPostSeriesRequest request)
        {
            var isExisted = await _unitOfWork.Series.IsPostInSeries(request.SeriesId, request.PostId);
            if (!isExisted)
            {
                return NotFound();
            }
            await _unitOfWork.Series.RemovePostToSeries(request.SeriesId, request.PostId);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }

        [HttpGet("post-series/{seriesId}")]
        [Authorize(Series.View)]
        public async Task<ActionResult<List<PostInListDto>>> GetPostsInSeries(Guid seriesId)
        {
            var posts = await _unitOfWork.Series.GetAllPostsInSeries(seriesId);
            return Ok(posts);
        }

        [HttpDelete]
        [Authorize(Series.Delete)]
        public async Task<IActionResult> DeleteSeries([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var post = await _unitOfWork.Series.GetByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }
                _unitOfWork.Series.Remove(post);
            }
            var result = await _unitOfWork.CompleteAsync();
            return result > 0 ? Ok() : BadRequest();
        }
    }
}
