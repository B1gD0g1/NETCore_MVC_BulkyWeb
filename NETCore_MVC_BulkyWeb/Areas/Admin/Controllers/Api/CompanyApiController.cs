using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.DTOModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [ApiController]
    [Route("admin/api/[controller]")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyApiController: ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll().ToList();

            var companyDtos = companyList.Select(c => new CompanyDTO
            {
                Id = c.Id,
                Name = c.Name,
                StreetAddress = c.StreetAddress,
                City = c.City,
                State = c.State,
                PostalCode = c.PostalCode,
                PhoneNumber = c.PhoneNumber
            }).ToList();

            //返回标准化JSON响应
            return Ok(new { data = companyDtos });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.Company.Get(p => p.Id == id);
            if (companyToBeDeleted is null)
            {
                return Ok(new { success = false, message = "删除失败！" });
            }

            _unitOfWork.Company.Remove(companyToBeDeleted);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true, message = "删除成功！" });
        }
    }
}
