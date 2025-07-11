using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.DTOModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [ApiController]
    [Route("admin/api/[controller]")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductApiController: ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductApiController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var productList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            var productDtos = productList.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Title = p.Title,
                Description = p.Description,
                ISBN = p.ISBN,
                Author = p.Author,
                ListPrice = p.ListPrice,
                ImageUrl = p.ImageUrl,
                Category = p.Category == null ? null : new CategoryDTO
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                }
            }).ToList();

            //返回标准化JSON响应
            return Ok(new { data = productDtos });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var productToBeDeleted = unitOfWork.Product.Get(p => p.ProductId == id);
            if (productToBeDeleted is null)
            {
                return Ok(new { success = false, message = "删除失败！" });
            }

            //删除旧图片
            var oldImagePath = Path
                .Combine(webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            unitOfWork.Product.Remove(productToBeDeleted);
            await unitOfWork.SaveAsync();

            return Ok(new { success = true, message = "删除成功！" });
        }
    }
}
