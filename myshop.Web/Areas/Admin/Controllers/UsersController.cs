using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.Utilities;
using System.Security.Claims;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles=SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            string? userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var users = _unitOfWork.ApplicationUser.GetAll(u => u.Id != userId).ToList();
            return View(users);
        }
        public IActionResult LockUnlock(string id)//LockUnlock
        {
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                //user is currently locked, we will unlock them
                user.LockoutEnd = DateTime.UtcNow;
            }
            else
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
            }
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
