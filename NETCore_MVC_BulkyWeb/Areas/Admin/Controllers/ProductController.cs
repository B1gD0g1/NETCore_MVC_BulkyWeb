using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.DTOModels;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var companyList = unitOfWork.Company.GetAll().ToList();
             
            return View(companyList);
        }

        public IActionResult UpdateAndInsert(int? id)
        {
            if (id is null || id == 0)
            {
                //创建 Create
                return View(new Company());
            }
            else
            {
                //更新 Update
                var companyObj = unitOfWork.Company.Get(c => c.Id == id);
                return View(companyObj);
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateAndInsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                if (companyObj.Id == 0)
                {
                    unitOfWork.Company.Add(companyObj);
                    TempData["success"] = "创建成功。";
                }
                else
                {
                    unitOfWork.Company.Update(companyObj);
                    TempData["success"] = "更新成功。";
                }

                await unitOfWork.SaveAsync();
                return RedirectToAction("Index", "Company");
            }
            else
            {
                TempData["error"] = "操作失败！";

                return View(companyObj);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var companyFromDb = unitOfWork.Company.Get(p => p.Id == id);
            if (companyFromDb == null)
            {
                return NotFound();
            }

            return View(companyFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var companyFromDb = unitOfWork.Company.Get(p => p.Id == id);

            if (companyFromDb is null)
            {
                return NotFound();
            }

            unitOfWork.Company.Remove(companyFromDb);
            await unitOfWork.SaveAsync();

            TempData["success"] = "产品删除成功。";
            return RedirectToAction("Index", "Company");
        }

        //#region API CALLS

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var companyList = unitOfWork.Company.GetAll(includeProperties: "Category").ToList();

        //    var companyDtos = companyList.Select(p => new CompanyDTO
        //    {
        //        CompanyId = p.CompanyId,
        //        Title = p.Title,
        //        Description = p.Description,
        //        ISBN = p.ISBN,
        //        Author = p.Author,
        //        ListPrice = p.ListPrice,
        //        ImageUrl = p.ImageUrl,
        //        Category = p.Category == null ? null : new CategoryDTO
        //        {
        //            Id = p.Category.Id,
        //            Name = p.Category.Name
        //        }
        //    }).ToList();

        //    //返回标准化JSON响应
        //    return Ok(new { data = companyDtos });
        //}

        //[HttpDelete]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    var companyToBeDeleted = unitOfWork.Company.Get(p => p.CompanyId == id);
        //    if (companyToBeDeleted is null)
        //    {
        //        return Json(new { success = false, message = "删除失败！" });
        //    }

        //    //删除旧图片
        //    var oldImagePath = Path
        //        .Combine(webHostEnvironment.WebRootPath, 
        //        companyToBeDeleted.ImageUrl.TrimStart('\\'));

        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }

        //    unitOfWork.Company.Remove(companyToBeDeleted);
        //    await unitOfWork.SaveAsync();

        //    return Json(new { success = true, message = "删除成功！" });
        //}

        //#endregion
    }
}
