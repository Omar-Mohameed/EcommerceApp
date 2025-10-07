using Microsoft.AspNetCore.Mvc;
using myshop.Business.Repositories;
using myshop.DataAccess.Implementation;
using myshop.Utilities;
using System.Security.Claims;

namespace myshop.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            /*
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var count = 0;
            if (userId != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                else
                {
                    count = unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId).Count();
                    HttpContext.Session.SetInt32(SD.SessionKey, count);
                }
            }
            else
            {
                HttpContext.Session.SetInt32(SD.SessionKey, 0);
            }
                return View(count);
            */


            int count = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                count = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count();
            }

            return View(count);
        }
    }
}
