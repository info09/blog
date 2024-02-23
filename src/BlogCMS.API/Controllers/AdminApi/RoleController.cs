using AutoMapper;
using BlogCMS.API.Extensions;
using BlogCMS.API.Filters;
using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.System.Roles;
using BlogCMS.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

        [HttpGet("{id}")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
        {
            var result = await _roleManager.FindByIdAsync(id.ToString());
            var data = _mapper.Map<RoleDto>(result);
            return Ok(data);
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
        [ValidateModel]
        public async Task<IActionResult> CreateRole([FromBody] CreateUpdateRoleRequest request)
        {
            await _roleManager.CreateAsync(new AppRole() { DisplayName = request.DisplayName, Name = request.Name, Id = Guid.NewGuid() });
            return new OkResult();
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.Roles.Edit)]
        [ValidateModel]
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

        [HttpGet("{roleId}/permissions")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PermissionDto>> GetAllRolePermissions(string roleId)
        {
            var model = new PermissionDto();
            var allPermissions = new List<RoleClaimsDto>();
            var types = typeof(Permissions).GetTypeInfo().DeclaredNestedTypes;
            foreach (var type in types)
            {
                allPermissions.GetPermissions(type);
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();
            model.RoleId = roleId;
            var claims = await _roleManager.GetClaimsAsync(role);
            var allClaimValues = allPermissions.Select(a => a.Value).ToList();
            var roleClaimValues = claims.Select(a => a.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permission in allPermissions)
            {
                if (authorizedClaims.Any(a => a == permission.Value))
                {
                    permission.Selected = true;
                }
            }
            model.RoleClaims = allPermissions;
            return Ok(model);
        }

        [HttpPut("permissions")]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> SavePermission([FromBody] PermissionDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
                return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaim(role, claim.Value);
            }
            return Ok();
        }
    }
}
