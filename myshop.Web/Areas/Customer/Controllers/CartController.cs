using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.Business.ViewModels;
using myshop.DataAccess.Implementation;
using myshop.Utilities;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, include: "Product"),
                OrderHeader = new OrderHeader()
            };
            foreach (var cart in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.totalCarts += (cart.Product.Price * cart.Count);
                ShoppingCartVM.OrderHeader.totalPrice += (cart.Count * cart.Product.Price);
            }
            return View(ShoppingCartVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, include: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PhoneNumber = string.IsNullOrEmpty(ShoppingCartVM.OrderHeader.ApplicationUser?.PhoneNumber) ? "N/A" : ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;


            foreach (var cart in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.totalCarts += (cart.Product.Price * cart.Count);
                ShoppingCartVM.OrderHeader.totalPrice += (cart.Count * cart.Product.Price);
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult POSTSummary(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var cartlist = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, include: "Product");
            //  Check لو السلة فاضية
            if (cartlist == null || !cartlist.Any())
            {
                // ممكن ترجعه على الصفحة الرئيسية أو cart/index
                TempData["Error"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction("Index", "Cart");
            }

            shoppingCartVM.CartsList = cartlist;

            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;

            foreach (var cart in shoppingCartVM.CartsList)
            {
                shoppingCartVM.OrderHeader.totalPrice += (cart.Product.Price * cart.Count);
            }
            unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            unitOfWork.Complete();
            // Order Details
            foreach (var cart in shoppingCartVM.CartsList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Product.Price,
                    Count = cart.Count
                };
                unitOfWork.OrderDetail.Add(orderDetail);
            }
            unitOfWork.Complete();

            // Stripe Settings
            var domain = $"{Request.Scheme}://{Request.Host}/";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in shoppingCartVM.CartsList)
            {
                var sessionlineoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionlineoption);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            shoppingCartVM.OrderHeader.SessionId = session.Id;

            unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approved, SD.Approved);
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingcarts = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            //HttpContext.Session.Clear();
            unitOfWork.ShoppingCart.RemoveRange(shoppingcarts);
            unitOfWork.Complete();
            return View(id);
        }
        public IActionResult Plus(int id)  // Cart ID 
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id);
            unitOfWork.ShoppingCart.IncreaseCount(cart, 1);
            unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int id)
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id);
            if (cart.Count <= 1)
            {
                unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                unitOfWork.ShoppingCart.DecreaseCount(cart, 1);
            }
            unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)  // Cart ID 
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == id);
            if(cart == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            unitOfWork.ShoppingCart.Remove(cart);
            unitOfWork.Complete();
            return Json(new { success = true, message = "Course deleted successfully!" });
        }
    }
}
