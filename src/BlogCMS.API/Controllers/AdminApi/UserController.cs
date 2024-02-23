using AutoMapper;
using BlogCMS.API.Extensions;
using BlogCMS.API.Filters;
using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.Models;
using BlogCMS.Core.Models.System.Users;
using BlogCMS.Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BlogCMS.Core.SeedWorks.Constants.Permissions;

namespace BlogCMS.API.Controllers.AdminApi
{
    [Route("api/admin/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UserController(Mapper mapper, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize(Users.View)]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var userDto = _mapper.Map<AppUser, UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles;
            return Ok(userDto);
        }

        [HttpGet("paging")]
        [Authorize(Users.View)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetAllUserPaging(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(i => i.FirstName.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        i.LastName.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        i.UserName.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        i.Email.ToLower().Contains(keyword.ToLower().Trim()));

            var totalRow = await query.CountAsync();

            query = query.OrderByDescending(i => i.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var pagedResult = new PagedResult<UserDto>
            {
                Results = await _mapper.ProjectTo<UserDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                PageSize = pageSize,
                RowCount = totalRow
            };

            return Ok(pagedResult);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Users.Create)]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request)
        {
            if(await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest();
            }

            if(await _userManager.FindByNameAsync(request.UserName) != null)
            {
                return BadRequest();
            }

            var user = _mapper.Map<CreateUserRequest, AppUser>(request);
            var result = await _userManager.CreateAsync(user, request.Password);
            if(result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(string.Join("<br>", result.Errors.Select(i => i.Description)));
        }

        [HttpPut("{id}")]
        [ValidateModel]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            _mapper.Map(request, user);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpPut("password-change-current-user")]
        [ValidateModel]
        public async Task<IActionResult> ChangeMyPassWord([FromBody] ChangeMyPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpDelete]
        [Authorize(Users.Delete)]
        public async Task<IActionResult> DeleteUsers([FromQuery] string[] ids)
        {
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                await _userManager.DeleteAsync(user);
            }
            return Ok();
        }

        [HttpPost("set-password/{id}")]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> SetPassword(Guid id, [FromBody] SetPasswordRequest model)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpPost("change-email/{id}")]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> ChangeEmail(Guid id, [FromBody] ChangeEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.Email);
            var result = await _userManager.ChangeEmailAsync(user, request.Email, token);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            return Ok();
        }

        [HttpPut("{id}/assign-users")]
        [ValidateModel]
        [Authorize(Users.Edit)]
        public async Task<IActionResult> AssignRolesToUser(string id, [FromBody] string[] roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removedResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var addedResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addedResult.Succeeded || !removedResult.Succeeded)
            {
                List<IdentityError> addedErrorList = addedResult.Errors.ToList();
                List<IdentityError> removedErrorList = removedResult.Errors.ToList();
                var errorList = new List<IdentityError>();
                errorList.AddRange(addedErrorList);
                errorList.AddRange(removedErrorList);

                return BadRequest(string.Join("<br/>", errorList.Select(x => x.Description)));
            }
            return Ok();
        }
    }
}
