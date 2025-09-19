using Microsoft.AspNetCore.Mvc;
using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.Business.ViewModels;

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
        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, include: "Category");
            if (product == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new()
            {
                Product = product,
                Count = 1,
            };
            return View(cart);
        }
    }
}
