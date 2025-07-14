using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.DTOModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [ApiController]
    [Route("admin/api/[controller]")]
    public class OrderApiController: ControllerBase
    {

        private readonly IUnitOfWork unitOfWork;

        public OrderApiController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("getall")]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = unitOfWork.OrderHeader
                    .GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                orderHeaderList = unitOfWork.OrderHeader
                    .GetAll(oh => oh.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList
                        .Where(oh => oh.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList
                        .Where(oh => oh.PaymentStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList
                        .Where(oh => oh.PaymentStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaderList = orderHeaderList
                        .Where(oh => oh.PaymentStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            //返回标准化JSON响应
            return Ok(new { data = orderHeaderList.ToList() });
        }

    }
}
