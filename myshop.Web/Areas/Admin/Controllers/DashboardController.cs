using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Repositories;
using myshop.DataAccess.Implementation;
using myshop.Utilities;
using Stripe;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public DashboardController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Orders = unitOfWork.OrderHeader.GetAll().Count();
            ViewBag.ApprovedOrders = unitOfWork.OrderHeader.GetAll(x => x.OrderStatus == SD.Approved).Count();
            ViewBag.Users = unitOfWork.ApplicationUser.GetAll().Count();
            ViewBag.Products = unitOfWork.Product.GetAll().Count();

            // Stripe Setup
            var totalRevenue = await GetTotalRevenueAsync();
            ViewBag.TotalStripeRevenue = totalRevenue.ToString("N2");
            return View();
        }

        private async Task<decimal> GetTotalRevenueAsync()
        {
            //StripeConfiguration.ApiKey = "sk_test_51SDl3tL2f4jnuDlHsySfi4a7Oj5oPSQKWSSKNMJ5qdYP8po6dkHp4Ssiol5ODpiuXc0rX6srCoMxgaq83ODLKY1Y00MfkvS2NG"; // ضع مفتاحك السري هنا

            // اقرأ المفتاح من appsettings
            var stripeKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = stripeKey;
            
            var chargeService = new ChargeService();
            var options = new ChargeListOptions
            {
                Limit = 100
            };

            var charges = await chargeService.ListAsync(options);

            decimal totalRevenue = 0m;

            foreach (var charge in charges)
            {
                if (charge.Paid && charge.Status == "succeeded")
                {
                    // خصم أي مبالغ تم استرجاعها
                    totalRevenue += (charge.Amount - charge.AmountRefunded) / 100m;
                }
            }

            return totalRevenue;
        }
    }
}
