using AutoMapper;
using BlogCMS.Core.SeedWorks;
using BlogCMS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.WebApp.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public NavigationViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _unitOfWork.PostCategories.GetAllAsync();
            var navItems = model.Select(i => new NavigationItemViewModel()
            {
                Slug = i.Slug,
                Name = i.Name,
                Children = model.Where(i => i.ParentId == i.Id).Select(i => new NavigationItemViewModel()
                {
                    Name = i.Name,
                    Slug = i.Slug
                }).ToList()
            }).ToList();
            return View(navItems);
        }
    }
}
