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
        public IActionResult ListByTag([FromRoute] string tagSlug, [FromQuery] int? page = 1)
        {
            return View();
        }

        [Route("post/{slug}")]
        public IActionResult Details([FromRoute] string slug)
        {
            return View();
        }
    }
}
