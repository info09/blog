using BlogCMS.Core.Models.Royalty;
using BlogCMS.Core.Models;
using BlogCMS.Core.SeedWorks;
using BlogCMS.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BlogCMS.Core.SeedWorks.Constants.Permissions;
using BlogCMS.Core.Domain.Royalty;
using BlogCMS.Data.Services;
using BlogCMS.API.Extensions;

namespace BlogCMS.API.Controllers.AdminApi
{
    [Route("api/admin/royalty")]
    [ApiController]
    public class RoyaltyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoyaltyService _royaltyService;

        public RoyaltyController(IUnitOfWork unitOfWork, IRoyaltyService royaltyService)
        {
            _royaltyService = royaltyService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("transaction-histories")]
        [Authorize(Royalty.View)]
        public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactionHistory(string? keyword, int fromMonth, int fromYear,
                                                                                            int toMonth, int toYear, 
                                                                                            int pageIndex, int pageSize = 10)
        {
            var result = await _unitOfWork.Transactions.GetAllPaging(keyword, fromMonth, fromYear, toMonth, toYear, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("Royalty-report-by-user")]
        [Authorize(Royalty.View)]
        public async Task<ActionResult<List<RoyaltyReportByUserDto>>> GetRoyaltyReportByUser(Guid? userId,
          int fromMonth, int fromYear, int toMonth, int toYear)
        {
            var result = await _royaltyService.GetRoyaltyReportByUserAsync(userId, fromMonth, fromYear, toMonth, toYear);
            return Ok(result);
        }

        [HttpGet("Royalty-report-by-month")]
        [Authorize(Royalty.View)]
        public async Task<ActionResult<List<RoyaltyReportByMonthDto>>> GetRoyaltyReportByMonth(Guid? userId,
         int fromMonth, int fromYear, int toMonth, int toYear)
        {
            var result = await _royaltyService.GetRoyaltyReportByMonthAsync(userId, fromMonth, fromYear, toMonth, toYear);
            return Ok(result);
        }

        [HttpPost("{userId}")]
        [Authorize(Royalty.Pay)]
        public async Task<IActionResult> PayRoyalty(Guid userId)
        {
            var fromUserId = User.GetUserId();
            await _royaltyService.PayRoyaltyForUserAsync(fromUserId, userId);
            return Ok();
        }
    }
}
