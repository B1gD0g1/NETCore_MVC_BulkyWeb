using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var productList = unitOfWork.Product.GetAll().ToList();

            return View(productList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Add(product);
                await unitOfWork.SaveAsync();
                TempData["success"] = "产品创建成功。";

                return RedirectToAction("Index", "Product");
            }
            else
            {
                TempData["error"] = "产品创建失败！";
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var productFromDb = unitOfWork.Product.Get(p => p.ProductId == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Update(product);
                await unitOfWork.SaveAsync();

                TempData["success"] = "产品更新成功。";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "产品更新失败！";
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }

            var productFromDb = unitOfWork.Product.Get(p => p.ProductId == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var productFromDb = unitOfWork.Product.Get(p => p.ProductId == id);

            if (productFromDb is null)
            {
                return NotFound();
            }

            unitOfWork.Product.Remove(productFromDb);
            await unitOfWork.SaveAsync();

            TempData["success"] = "产品删除成功。";
            return RedirectToAction("Index", "Product");
        }
    }
}
