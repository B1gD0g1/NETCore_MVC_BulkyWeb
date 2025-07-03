using Microsoft.AspNetCore.Mvc;
using NETCore_MVC_BulkyWeb.Data;
using NETCore_MVC_BulkyWeb.Models;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryController(ApplicationDbContext DbContext)
        {
            dbContext = DbContext;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = dbContext.Categories.ToList();

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
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();

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

            Category? categoryFromDb = dbContext.Categories.Find(id);//首先检查DbContext的本地缓存（内存中），如果找到就直接返回，不访问数据库
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
                dbContext.Categories.Update(category);
                await dbContext.SaveChangesAsync();

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

            Category? categoryFromDb = dbContext.Categories.Find(id);//首先检查DbContext的本地缓存（内存中），如果找到就直接返回，不访问数据库

            if (categoryFromDb is null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var categoryToDelete = dbContext.Categories.Find(id);

            if (categoryToDelete is null)
            {
                return NotFound();
            }

            dbContext.Categories.Remove(categoryToDelete);
            await dbContext.SaveChangesAsync();

            TempData["success"] = "类别删除成功！";

            return RedirectToAction("Index", "Category");
        }
    }
}
