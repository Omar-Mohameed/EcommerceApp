using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Repositories;
using myshop.Business.ViewModels;
using System.Security.Claims;

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
                CartsList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, include: "Product")
            };
            foreach (var cart in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.totalCarts += (cart.Product.Price * cart.Count);
            }
            return View(ShoppingCartVM);
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
