using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.DataAccess.Data;
using myshop.DataAccess.Implementation;
using myshop.Utilities;


namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // it protected from CSRF (Cross-Site Request Forgery) hacking
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Item Has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        #region EDIT
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var categoryFromDataBase = _unitOfWork.Category.GetFirstOrDefault(x=>x.Id == id);
            return View(categoryFromDataBase);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
                TempData["Update"] = "Item Has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        #endregion

        #region Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var categoryFromDataBase = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            return View(categoryFromDataBase);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {
            var categoryFromDataBase = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (categoryFromDataBase == null) return NotFound();

            _unitOfWork.Category.Remove(categoryFromDataBase);
            _unitOfWork.Complete();
            TempData["Delete"] = "Item Has Deleted Successfully";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
