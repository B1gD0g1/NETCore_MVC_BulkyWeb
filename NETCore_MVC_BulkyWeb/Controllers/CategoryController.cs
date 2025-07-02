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

                return RedirectToAction("Index", "Category");
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
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Category");
            }

            return View(category);
        }
    }
}
