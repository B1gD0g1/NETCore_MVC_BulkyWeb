using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryRepository.GetAll().ToList();

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
                _categoryRepository.Add(category);
                await _categoryRepository.SaveAsync();

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

            Category? categoryFromDb = _categoryRepository.Get(c => c.Id == id);
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
                _categoryRepository.Update(category);
                await _categoryRepository.SaveAsync();

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

            Category? categoryFromDb = _categoryRepository.Get(c => c.Id == id);

            if (categoryFromDb is null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var categoryToDelete =  _categoryRepository.Get(c => c.Id == id);

            if (categoryToDelete is null)
            {
                return NotFound();
            }

            _categoryRepository.Remove(categoryToDelete);
            await _categoryRepository.SaveAsync();

            TempData["success"] = "类别删除成功！";

            return RedirectToAction("Index", "Category");
        }
    }
}
