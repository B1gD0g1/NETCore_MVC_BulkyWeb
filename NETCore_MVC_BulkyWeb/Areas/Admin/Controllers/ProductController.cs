using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var productList = unitOfWork.Product.GetAll().ToList();

            return View(productList);
        }

        public IActionResult UpdateAndInsert(int? id)
        {
            IEnumerable<SelectListItem> categoryList = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            ProductViewModel productViewModel = new()
            {
                Product = new Product(),
                CatetoryList = categoryList
            };

            if (id is null || id == 0)
            {
                //创建 Create
                return View(productViewModel);
            }
            else
            {
                //更新 Update
                var productForId = unitOfWork.Product.Get(p => p.ProductId == id);
                if (productForId is null)
                {
                    return NotFound();
                }

                productViewModel.Product = productForId;
                return View(productViewModel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateAndInsert(ProductViewModel productViewModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;

                if (file is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
                }

                unitOfWork.Product.Add(productViewModel.Product);
                await unitOfWork.SaveAsync();
                TempData["success"] = "产品创建成功。";

                return RedirectToAction("Index", "Product");
            }
            else
            {
                IEnumerable<SelectListItem> categoryList = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                productViewModel.CatetoryList = categoryList;

                TempData["error"] = "产品创建失败！";

                return View(productViewModel);
            }
        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id is null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    var productFromDb = unitOfWork.Product.Get(p => p.ProductId == id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productFromDb);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        unitOfWork.Product.Update(product);
        //        await unitOfWork.SaveAsync();

        //        TempData["success"] = "产品更新成功。";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        TempData["error"] = "产品更新失败！";
        //    }

        //    return View();
        //}

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
