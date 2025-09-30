using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Models;
using myshop.Business.Repositories;
using System.Security.Claims;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }
        [HttpGet("Customer/Home/Details/{id?}")]
        public IActionResult Details([FromRoute] int? id, [FromQuery] int? ProductId)
        {
            var finalId = id ?? ProductId;
            var product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == finalId, include: "Category");
            if (product == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new()
            {
                ProductId = product.Id,
                Product = product,
                Count = 1,
            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return Unauthorized(); // لو المستخدم مش مسجل دخول
            }

            cart.ApplicationUserId = claim.Value;

            if (cart.Count <= 0)
            {
                ModelState.AddModelError("Count", "Quantity must be greater than zero.");
                return RedirectToAction(nameof(Details), new { id = cart.ProductId });
            }

            var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == cart.ApplicationUserId && u.ProductId == cart.ProductId);
            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }
            else
            {
                cartFromDb.Count += cart.Count;
            }
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
