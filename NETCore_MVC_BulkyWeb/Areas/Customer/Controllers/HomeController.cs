using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NETCore_MVC_BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var productList = unitOfWork.Product.GetAll(includeProperties: "Category");

            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            var shoppingCart = new ShoppingCart()
            {
                Product = unitOfWork.Product
                .Get(p => p.ProductId == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            var shoppingCartFromDb = unitOfWork.ShoppingCart
                .Get(sc => sc.ApplicationUserId == userId && sc.ProductId == shoppingCart.ProductId);

            if (shoppingCartFromDb is not null)
            {
                //购物车存在这种产品，所以这时候不添加新的，而是更新本来的数量
                shoppingCartFromDb.Count += shoppingCart.Count;
                unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            }
            else
            {
                //购物车不存在这种产品，所以添加新的
                unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            await unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
