using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Threading.Tasks;

namespace NETCore_MVC_BulkyWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderViewModel = new OrderViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader
                    .Get(oh => oh.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail
                    .GetAll(od => od.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderViewModel); //UpdateOrderDetail
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
        public async Task<IActionResult> UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader
                .Get(oh => oh.Id == OrderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");

            orderHeaderFromDb.Name = OrderViewModel.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;
            orderHeaderFromDb.State = OrderViewModel.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "订单详情更新成功。";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
        public async Task<IActionResult> StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderViewModel.OrderHeader.Id, SD.StatusInProcess);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "订单详情更新成功。";

            return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
        public async Task<IActionResult> ShipOrder()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader
                .Get(oh => oh.Id == OrderViewModel.OrderHeader.Id);

            orderHeaderFromDb.ShippingDate = DateTime.Now;
            orderHeaderFromDb.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
            orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;

            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "订单成功发货。";

            return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
        public async Task<IActionResult> CancelOrder()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader
                .Get(oh => oh.Id == OrderViewModel.OrderHeader.Id);

            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusApproved)
            {
                //已经支付了，需要退款
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = await service.CreateAsync(options);

                _unitOfWork.OrderHeader
                    .UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader
                    .UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
            }

            await _unitOfWork.SaveAsync();
            TempData["success"] = "订单取消成功。";
            return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> Details_PAY_NOW()
        {
            OrderViewModel.OrderHeader = _unitOfWork.OrderHeader
                .Get(oh => oh.Id == OrderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderViewModel.OrderDetails = _unitOfWork.OrderDetail
                .GetAll(od => od.OrderHeaderId == OrderViewModel.OrderHeader.Id, includeProperties: "Product");

            var domain = "https://localhost:7262/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderViewModel.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderViewModel.OrderHeader.Id}",
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

            foreach (var item in OrderViewModel.OrderDetails)
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

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderViewModel.OrderHeader.Id,
                session.Id, session.PaymentIntentId);
            await _unitOfWork.SaveAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {
            var orderHeader = _unitOfWork.OrderHeader
                .Get(o => o.Id == orderHeaderId);

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //这是公司/企业账户的订单
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId,
                        session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    await _unitOfWork.SaveAsync();
                }
            }

            return View(orderHeaderId);
        }
    }
}
