using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Repositories;
using myshop.Business.ViewModels;
using myshop.DataAccess.Implementation;
using myshop.Utilities;
using Stripe;
using System.Configuration;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        [BindProperty]
        public OrderVM orderVM { get; set; }
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
        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM() 
            { 
                OrderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderid,include: "ApplicationUser"),
                OrderDetails = unitOfWork.OrderDetail.GetAll(o => o.OrderHeaderId == orderid, include: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails() // orderVM is coming from [BindProperty]
        {
            var orderHeaderFromDb = unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVM.OrderHeader.Id);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
                orderHeaderFromDb.Address = orderVM.OrderHeader.Address;
                orderHeaderFromDb.City = orderVM.OrderHeader.City;
                orderHeaderFromDb.PhoneNumber = string.IsNullOrEmpty(orderVM.OrderHeader.PhoneNumber) ? "N/A" : orderVM.OrderHeader.PhoneNumber;

                if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
                {
                    orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
                }
                if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
                {
                    orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                }
                unitOfWork.OrderHeader.Update(orderHeaderFromDb);
                unitOfWork.Complete();
                TempData["Update"] = "Order details updated successfully.";
                return RedirectToAction("Details", "Order", new { orderid = orderHeaderFromDb.Id });
            }
            TempData["Delete"] = "Error while updating the order details.";
            return RedirectToAction("Details", "Order", new { orderid = orderHeaderFromDb.Id });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProccess()
        {
            unitOfWork.OrderHeader.UpdateOrderStatus(orderVM.OrderHeader.Id, SD.InProcess);
            unitOfWork.Complete();

            TempData["Update"] = "Order status updated successfully.";
            return RedirectToAction("Details", "Order", new { orderid = orderVM.OrderHeader.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip()
        {
            var orderHeaderFromDb = unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVM.OrderHeader.Id);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
                orderHeaderFromDb.OrderStatus = SD.Shipped;
                orderHeaderFromDb.ShippingDate = DateTime.Now;
                unitOfWork.OrderHeader.Update(orderHeaderFromDb);
                unitOfWork.Complete();
                TempData["Update"] = "Order has shipped successfully.";
                return RedirectToAction("Details", "Order", new { orderid = orderHeaderFromDb.Id });
            }
            TempData["Delete"] = "Error while updating the order status.";
            return RedirectToAction("Details", "Order", new { orderid = orderHeaderFromDb.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderHeaderFromDb = unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderVM.OrderHeader.Id);
            if(orderHeaderFromDb.PaymentStatus == SD.Approved)
            {
                // TODO : Refund the amount
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                unitOfWork.OrderHeader.UpdateOrderStatus(orderVM.OrderHeader.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                unitOfWork.OrderHeader.UpdateOrderStatus(orderVM.OrderHeader.Id, SD.Cancelled,SD.Cancelled);
            }
            unitOfWork.Complete();

            TempData["Update"] = "Order has cancelled successfully.";
            return RedirectToAction("Details", "Order", new { orderid = orderVM.OrderHeader.Id });
        }
    }

}
