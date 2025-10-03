using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Repositories;
using myshop.DataAccess.Implementation;
using myshop.Utilities;
using System.Configuration;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            var orderHeader = unitOfWork.OrderHeader.GetAll(include: "ApplicationUser");
            return Json(new { data = orderHeader });
        }
    }
}
