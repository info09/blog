using AutoMapper;
using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.System;
using BlogCMS.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.API.Controllers.AdminApi
{
    [Route("api/admin/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _mapper = mapper;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
        {
            var model = await _mapper.ProjectTo<RoleDto>(_roleManager.Roles).ToListAsync();
            return Ok(model);
        }

        [HttpGet("paging")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PagedResult<RoleDto>>> GetRolesPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(i => i.Name.ToLower().Contains(keyword.ToLower().Trim()));

            var totalRow = await query.CountAsync();
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var data = await _mapper.ProjectTo<RoleDto>(query).ToListAsync();
            var paginationResult = new PagedResult<RoleDto>
            {
                Results = data,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                RowCount = totalRow,
            };
            return Ok(paginationResult);
        }

        [HttpPost]
        [Authorize(Permissions.Roles.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateUpdateRoleRequest request)
        {
            await _roleManager.CreateAsync(new AppRole() { DisplayName = request.DisplayName, Name = request.Name, Id = Guid.NewGuid() });
            return new OkResult();
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] CreateUpdateRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return NotFound();

            role.Name = request.Name;
            role.DisplayName = request.DisplayName;

            await _roleManager.UpdateAsync(role);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Permissions.Roles.Delete)]
        public async Task<IActionResult> DeleteRoles([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null) return NotFound();
                await _roleManager.DeleteAsync(role);
            }

            return Ok();
        }
    }
}
