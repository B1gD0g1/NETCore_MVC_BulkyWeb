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
            var productList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
             
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

                    if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))
                    {
                        //删除旧图片
                        var oldImagePath = Path
                            .Combine(wwwRootPath, productViewModel.Product.ImageUrl
                            .TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productViewModel.Product.ProductId == 0)
                {
                    unitOfWork.Product.Add(productViewModel.Product);
                }
                else
                {
                    unitOfWork.Product.Update(productViewModel.Product);
                }

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
