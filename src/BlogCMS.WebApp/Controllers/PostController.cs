using BlogCMS.Core.SeedWorks;
using BlogCMS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.WebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("posts")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("posts/{categorySlug}")]
        public async Task<IActionResult> ListByCategory([FromRoute] string categorySlug, [FromQuery] int page = 1, int pageSize = 2)
        {
            var posts = await _unitOfWork.Posts.GetPostByCategoryPaging(categorySlug, page, pageSize);
            var category = await _unitOfWork.PostCategories.GetBySlug(categorySlug);
            return View(new PostListByCategoryViewModel()
            {
                Posts = posts,
                Category = category
            });
        }

        [Route("tag/{tagSlug}")]
        public async Task<IActionResult> ListByTag([FromRoute] string tagSlug, [FromQuery] int page = 1)
        {
            var tags = await _unitOfWork.Tags.GetBySlug(tagSlug);
            var posts = await _unitOfWork.Posts.GetPostByTagPaging(tagSlug, page, 2);
            return View(new PostListByTagViewModel()
            {
                Posts = posts,
                Tag = tags
            });
        }

        [Route("post/{slug}")]
        public async Task<IActionResult> Details([FromRoute] string slug)
        {
            var post = await _unitOfWork.Posts.GetBySlug(slug);
            var category = await _unitOfWork.PostCategories.GetBySlug(post.CategorySlug);
            var tags = await _unitOfWork.Posts.GetTagObjectsByPostId(post.Id);

            return View(new PostDetailViewModel()
            {
                Post = post,
                Category = category,
                Tags = tags
            });
        }
    }
}
