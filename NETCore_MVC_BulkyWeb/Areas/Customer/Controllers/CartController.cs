using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart
                    .GetAll(sc => sc.ApplicationUserId == userId, includeProperties: "Product")
            };

            foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartViewModel.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult Summary()
        {
            return View();
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);

            if (shoppingCartFromDb is not null)
            {
                shoppingCartFromDb.Count += 1;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);

            //if (shoppingCartFromDb is not null)
            //{
            //    if (shoppingCartFromDb.Count <= 1)
            //    {
            //        //将该产品从购物车中移除
            //        _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
            //    }
            //    else
            //    {
            //        shoppingCartFromDb.Count -= 1;
            //        _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
            //    }

            //    await _unitOfWork.SaveAsync();
            //}

            //当数量等于1时，跳出该函数，而不是将产品删除。
            if (shoppingCartFromDb is not null && shoppingCartFromDb.Count > 1)
            {
                shoppingCartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);

            if (shoppingCartFromDb is not null)
            {
                //移除该产品
                _unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);

                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            // 根据购物车中的商品数量返回相应的价格
            if (shoppingCart.Product is not null)
            {
                if (shoppingCart.Count <= 50)
                {
                    return shoppingCart.Product.Price;
                }
                else
                {
                    if (shoppingCart.Count <= 100)
                    {
                        return shoppingCart.Product.Price50;
                    }
                    else
                    {
                        return shoppingCart.Product.Price100;
                    }
                }
            }

            return 0;
        } 
    }
}
