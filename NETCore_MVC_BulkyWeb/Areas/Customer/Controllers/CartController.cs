using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
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
                    .GetAll(sc => sc.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart
                    .GetAll(sc => sc.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(a => a.Id == userId);

            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart
                    .GetAll(sc => sc.ApplicationUserId == userId, includeProperties: "Product");
             
            ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser
                .Get(a => a.Id == userId);

            foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser?.CompanyId.GetValueOrDefault() == 0)
            {
                //这是一个普通的客户帐户，我们需要捕获付款
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //这是一个公司账户
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                await _unitOfWork.SaveAsync();
            }

            if (applicationUser?.CompanyId.GetValueOrDefault() == 0)
            {
                //普通账户，进入支付流程
                var domain = "https://localhost:7262/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?orderId={ShoppingCartViewModel.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    PaymentMethodTypes = new List<string>
                    {
                        "card",       // 这包括VISA和万事达等信用卡
                        "alipay",    // 支付宝
                        "wechat_pay"  // 微信支付
                    },
                    PaymentMethodOptions = new SessionPaymentMethodOptionsOptions
                    {
                        // 微信支付必须设置 client: "web"
                        WechatPay = new SessionPaymentMethodOptionsWechatPayOptions
                        {
                            Client = "web" // 适用于网页版微信支付
                        },
                    }
                };

                foreach (var item in ShoppingCartViewModel.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // 比如：¥30.50 => 3050
                            Currency = "cny",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product?.Title,
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id,
                    session.Id, session.PaymentIntentId);
                await _unitOfWork.SaveAsync();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation),
                new { orderId = ShoppingCartViewModel.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var orderHeader = _unitOfWork.OrderHeader
                .Get(o => o.Id == orderId, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //这是客户账户的订单
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderId,
                        session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusApproved, SD.PaymentStatusApproved);
                    await _unitOfWork.SaveAsync();
                }
            }

            //清空购物车
            var shoppingCartList = _unitOfWork.ShoppingCart
                .GetAll(sc => sc.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartList);
            await _unitOfWork.SaveAsync();

            return View(orderId);
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
