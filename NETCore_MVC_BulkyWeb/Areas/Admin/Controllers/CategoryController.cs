using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();

            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "类别创建成功！";

                return RedirectToAction("Index", "Category");
            }
            else
            {
                TempData["error"] = "类别创建失败！";
            }

            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDb = _unitOfWork.Category.Get(c => c.Id == id);
            //Category? categoryFromDb = dbContext.Categories.Find(id);//首先检查DbContext的本地缓存（内存中），如果找到就直接返回，不访问数据库
            //Category? categoryFromDb = dbContext.Categories.FirstOrDefault(c => c.Id == id);
            //Category? categoryFromDb = dbContext.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (categoryFromDb is null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "类别更新成功！";

                return RedirectToAction("Index", "Category");
            }
            else
            {
                TempData["error"] = "类别更新失败！";
            }

            return View(category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDb = _unitOfWork.Category.Get(c => c.Id == id);

            if (categoryFromDb is null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var categoryToDelete = _unitOfWork.Category.Get(c => c.Id == id);

            if (categoryToDelete is null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(categoryToDelete);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "类别删除成功！";

            return RedirectToAction("Index", "Category");
        }
    }
}
