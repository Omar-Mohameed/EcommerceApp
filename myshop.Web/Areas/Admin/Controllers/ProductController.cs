using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared;
using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.Business.ViewModels;
using myshop.DataAccess.Data;
using myshop.DataAccess.Implementation;


namespace myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region View All Product In Data TAble
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(include:"Category");
            return Json(new {data =  products});
        }
        #endregion

        #region Create Product
        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                categoryList = _unitOfWork.Category.GetAll().Select(x =>
                    new SelectListItem(x.Name,x.Id.ToString())
                )
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // it protected from CSRF (Cross-Site Request Forgery) hacking
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if(ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Images/Products");
                    var ext = Path.GetExtension(file.FileName);

                    using(var filestream = new FileStream(Path.Combine(Upload,filename+ext),FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }

                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Item Has Created Successfully";
                return RedirectToAction("Index");
            }
            productVM.categoryList = _unitOfWork.Category.GetAll().Select(x =>
                   new SelectListItem(x.Name, x.Id.ToString())
               );
            return View(productVM);
        }
        #endregion

        #region EDIT
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var productFromDataBase = _unitOfWork.Product.GetFirstOrDefault(x=>x.Id == id);

            ProductVM productVM = new ProductVM()
            {
                Product = productFromDataBase,
                categoryList = _unitOfWork.Category.GetAll().Select(x =>
                    new SelectListItem(x.Name, x.Id.ToString())
                )
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            //ModelState.Remove("file");

            if (file != null)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString();
                var Upload = Path.Combine(rootPath, @"Images/Products");
                var ext = Path.GetExtension(file.FileName);

                // Delete Old Image
                if (productVM.Product.Img != null)
                {
                    var oldimg = Path.Combine(rootPath, productVM.Product.Img.TrimStart('\\'));
                    if (System.IO.File.Exists(oldimg))
                    {
                        System.IO.File.Delete(oldimg);
                    }
                }

                using (var filestream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                {
                    file.CopyTo(filestream);
                }
                productVM.Product.Img = @"Images\Products\" + filename + ext;

            }
            if(ModelState.IsValid)
            {
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Item Has Updated Successfully";
                return RedirectToAction("Index");
            }
            productVM.categoryList = _unitOfWork.Category.GetAll().Select(x =>
                    new SelectListItem(x.Name, x.Id.ToString())
                );
            return View(productVM);
        }
        #endregion

        #region Delete
        [HttpPost]
        public IActionResult DeleteProduct(int? id)
        {
            var productFromDataBase = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (productFromDataBase == null)
            {
                return Json(new {success  = false,message="Error While Deleting"});
            }

            _unitOfWork.Product.Remove(productFromDataBase);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productFromDataBase.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Item Has Been Deleted" });
        }
        #endregion
    }
}
